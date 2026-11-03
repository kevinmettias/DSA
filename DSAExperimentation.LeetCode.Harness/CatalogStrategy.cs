using System.Reflection;
using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Harness;

// One strategy of an enrolled solution as the catalog harness runs it outside a benchmark. It is found by
// the rules CatalogBenchmarkGenerator applies at build time - a non-private static <Operation>By<Strategy>
// method (ARCHITECTURE 17.3) with exactly one overload a case can feed - and called by reflection on one
// case at a time, its answer written in LeetCode's notation. A solution the generator would refuse is
// refused here too, by name, so the two never disagree about which strategies an enrolled solution has.
internal sealed class CatalogStrategy
{
    private const string StrategyInfix = "By";

    // The harness compiles against LeetCode's conventions, never against the solutions it runs, so it
    // loads their assembly by name. Every harness that runs one references the solutions project.
    private const string SolutionsAssemblyName = "DSAExperimentation.LeetCode.Solutions";

    private const BindingFlags DeclaredStatics =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private readonly MethodInfo _method;
    private readonly Type[] _inputTypes;

    public Type Solution { get; }

    public string Name => _method.Name;

    // The solutions project: every solution class, enrolled or not.
    public static Assembly SolutionsAssembly { get; } = Assembly.Load(SolutionsAssemblyName);

    private CatalogStrategy(Type solution, MethodInfo method)
    {
        Solution = solution;
        _method = method;
        _inputTypes = [.. method.GetParameters().Where(parameter => !parameter.IsOut).Select(parameter => parameter.ParameterType)];
    }

    // Every solution class in the solutions project that names its LeetCode question, by full name.
    public static IReadOnlyList<Type> EnrolledSolutions() =>
        [.. SolutionsAssembly.GetTypes()
            .Where(type => type.IsDefined(typeof(LeetCodeQuestionAttribute)))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)];

    // The solution's strategies in declaration order, each the one overload of its name a case can feed.
    public static IReadOnlyList<CatalogStrategy> Of(Type solution) =>
        [.. solution.GetMethods(DeclaredStatics)
            .Where(IsStrategy)
            .OrderBy(method => method.MetadataToken)
            .GroupBy(method => method.Name)
            .Select(overloads => new CatalogStrategy(solution, TheBoundOverload(solution, [.. overloads])))];

    private static MethodInfo TheBoundOverload(Type solution, List<MethodInfo> overloads)
    {
        var bound = overloads.Where(IsBindable).ToList();

        if (bound.Count != 1)
        {
            throw new InvalidOperationException(
                $"{solution.Name}.{overloads[0].Name} has {bound.Count} overloads a case can feed, and a catalog arm needs exactly one.");
        }

        return bound[0];
    }

    // The catalog's cases read into this strategy's parameter types, fresh on every call, so a strategy
    // that rewrites its input rewrites only its own copy.
    public CatalogColumns Columns() => CatalogColumns.For(Solution, _inputTypes);

    // The answer to the case at caseIndex of columns: the return value, or for a Try strategy the outputs
    // it found, LeetCode's [] when it found none.
    public JsonNode AnswerTo(CatalogColumns columns, int caseIndex)
    {
        var parameters = _method.GetParameters();
        var arguments = ArgumentsFor(parameters, columns, caseIndex);
        var returned = _method.Invoke(null, BindingFlags.DoNotWrapExceptions, binder: null, arguments, culture: null);
        var outputs = parameters.Where(parameter => parameter.IsOut).Select(parameter => arguments[parameter.Position]).ToList();

        return NotationOf(returned, outputs);
    }

    // A case's inputs at the positions of the parameters they fill; each out parameter's slot stays empty
    // for the strategy to write.
    private static object?[] ArgumentsFor(ParameterInfo[] parameters, CatalogColumns columns, int caseIndex)
    {
        var arguments = new object?[parameters.Length];
        var input = 0;

        foreach (var parameter in parameters.Where(parameter => !parameter.IsOut))
        {
            arguments[parameter.Position] = columns.ValueAt(input++, caseIndex);
        }

        return arguments;
    }

    private static JsonNode NotationOf(object? returned, List<object?> outputs)
    {
        if (outputs.Count == 0)
        {
            return AnswerNotation.Of(returned);
        }

        if (returned is not true)
        {
            return AnswerNotation.NoneFound();
        }

        return AnswerNotation.OfFound(outputs);
    }

    private static bool IsStrategy(MethodInfo method) =>
        (method.IsPublic || method.IsAssembly) && !method.IsSpecialName && IsStrategyNamed(method.Name);

    // A `By` after the first character, starting a new word.
    private static bool IsStrategyNamed(string name)
    {
        for (var index = name.IndexOf(StrategyInfix, StringComparison.Ordinal);
            index > 0;
            index = name.IndexOf(StrategyInfix, index + 1, StringComparison.Ordinal))
        {
            var start = index + StrategyInfix.Length;

            if (start < name.Length && char.IsUpper(name[start]))
            {
                return true;
            }
        }

        return false;
    }

    // The generator's rule, over run-time types: not generic, an answer to store by value, and every input
    // one LeetCode's notation can be read into.
    private static bool IsBindable(MethodInfo method)
    {
        if (method.IsGenericMethodDefinition || HasNoAnswerToStore(method.ReturnType))
        {
            return false;
        }

        return method.GetParameters().All(parameter => parameter.IsOut || IsReadableInput(parameter.ParameterType));
    }

    // A void method answers by mutating its input, and a ref return or a ref struct cannot sit in an array.
    private static bool HasNoAnswerToStore(Type returnType) =>
        returnType == typeof(void) || returnType.IsByRef || returnType.IsByRefLike;

    private static bool IsReadableInput(Type parameterType) => !parameterType.IsByRef && NotationReader.CanRead(parameterType);
}
