using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Harness;

// One strategy of an enrolled solution as the catalog harness runs it outside a benchmark. It is found by
// the rules CatalogBenchmarkGenerator applies at build time - a non-private static <Operation>By<Strategy>
// method (ARCHITECTURE 17.3) with exactly one overload a case can feed - and called by reflection on one
// case at a time, its answer written in LeetCode's notation. A solution the generator would refuse is
// refused here too, by name, so the two never disagree about which strategies an enrolled solution has.
// A design question's strategy is a factory taking the constructor's inputs, and answers a case by
// replaying its call script on the object it makes (CatalogCalls).
internal sealed class CatalogStrategy
{
    private const string StrategyInfix = "By";

    // The harness compiles against LeetCode's conventions, never against the solutions it runs, so it
    // loads their assembly by name. Every harness that runs one references the solutions project.
    private const string SolutionsAssemblyName = "DSAExperimentation.LeetCode.Solutions";

    private const BindingFlags DeclaredStatics =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private readonly MethodInfo _method;
    private readonly bool _answersInPlace;
    private readonly Type[] _inputTypes;

    // For a design question, the methods answering its calls on the object this strategy makes; null for a
    // function question.
    private readonly CatalogCalls? _calls;

    public Type Solution { get; }

    public string Name => _method.Name;

    // The solutions project: every solution class, enrolled or not.
    public static Assembly SolutionsAssembly { get; } = Assembly.Load(SolutionsAssemblyName);

    private CatalogStrategy(Type solution, MethodInfo method, LeetCodeQuestionAttribute? question, CatalogCalls? calls)
    {
        Solution = solution;
        _method = method;
        _answersInPlace = question?.AnswersInPlace ?? false;
        _inputTypes = [.. method.GetParameters().Where(parameter => !parameter.IsOut).Select(parameter => parameter.ParameterType)];
        _calls = calls;
    }

    // Every solution class in the solutions project that names its LeetCode question, by full name.
    public static IReadOnlyList<Type> EnrolledSolutions() =>
        [.. SolutionsAssembly.GetTypes()
            .Where(type => type.IsDefined(typeof(LeetCodeQuestionAttribute)))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)];

    // The solution's strategies in declaration order, each the one overload of its name a case can feed, or
    // for a design question the one that makes its object.
    public static IReadOnlyList<CatalogStrategy> Of(Type solution)
    {
        var question = solution.GetCustomAttribute<LeetCodeQuestionAttribute>();
        var design = DesignSignatureOf(question);

        return [.. solution.GetMethods(DeclaredStatics)
            .Where(IsStrategy)
            .OrderBy(method => method.MetadataToken)
            .GroupBy(method => method.Name)
            .Select(overloads => StrategyOf(solution, [.. overloads], question, design))];
    }

    // The signature of the solution's question when it is a design question; null for a function question or
    // a solution that names none.
    private static LeetCodeSignature? DesignSignatureOf(LeetCodeQuestionAttribute? question)
    {
        if (question is null)
        {
            return null;
        }

        var signature = LeetCodeCatalog.Question(question.TitleSlug).Signature;

        if (!signature.IsDesign)
        {
            return null;
        }

        return signature;
    }

    private static CatalogStrategy StrategyOf(Type solution, List<MethodInfo> overloads, LeetCodeQuestionAttribute? question, LeetCodeSignature? design)
    {
        if (question is null || design is null)
        {
            return new CatalogStrategy(solution, TheBoundOverload(solution, overloads, question), question, calls: null);
        }

        var factory = TheFactory(solution, overloads, design);
        var calls = CatalogCallBinder.Bind(factory, question.Calls, design.Methods);

        return new CatalogStrategy(solution, factory, question, calls);
    }

    // The one overload a case can feed; of several, the one LeetCode's signature shapes (LeetCodeShape).
    private static MethodInfo TheBoundOverload(Type solution, List<MethodInfo> overloads, LeetCodeQuestionAttribute? question)
    {
        IReadOnlyList<MethodInfo> bound = [.. overloads.Where(method => IsBindable(method, question))];

        if (bound.Count > 1 && question is not null)
        {
            var declared = LeetCodeCatalog.Question(question.TitleSlug).Signature.Parameters.Select(parameter => parameter.Type).ToList();
            bound = LeetCodeShape.Narrowed(bound, declared);
        }

        if (bound.Count != 1)
        {
            throw new InvalidOperationException(
                $"{solution.Name}.{overloads[0].Name} has {bound.Count} overloads a case can feed, and a catalog arm needs exactly one.");
        }

        return bound[0];
    }

    // The generator's rule, over run-time types: not generic, an answer to store by value - or, for a
    // question answered in place, no return value and a first input to answer with - and every input one
    // LeetCode's notation can be read into.
    private static bool IsBindable(MethodInfo method, LeetCodeQuestionAttribute? question)
    {
        var parameters = method.GetParameters();
        var isAnsweredInPlace = question?.AnswersInPlace ?? false;
        var answers = isAnsweredInPlace ? IsInPlaceAnswer(method, parameters) : !HasNoAnswerToStore(method.ReturnType);

        if (method.IsGenericMethodDefinition || !answers)
        {
            return false;
        }

        return parameters.All(parameter => parameter.IsOut || IsReadableInput(parameter.ParameterType));
    }

    // Returns nothing, and leaves its answer in a first parameter that a case fills.
    private static bool IsInPlaceAnswer(MethodInfo method, ParameterInfo[] parameters) =>
        method.ReturnType == typeof(void) && parameters is [{ IsOut: false }, ..];

    // The one overload that makes a design question's object: taking the constructor's inputs, each of them
    // readable, and returning the object its calls are made on.
    private static MethodInfo TheFactory(Type solution, List<MethodInfo> overloads, LeetCodeSignature design)
    {
        var bound = overloads.Where(method => IsFactory(method, design.ScriptedConstructorParameters.Count)).ToList();

        if (bound.Count != 1)
        {
            throw new InvalidOperationException(
                $"{solution.Name}.{overloads[0].Name} has {bound.Count} overloads that make the question's object from its constructor's inputs, and a catalog arm needs exactly one.");
        }

        return bound[0];
    }

    private static bool IsFactory(MethodInfo method, int constructorInputs)
    {
        var parameters = method.GetParameters();
        var makesNoInstanceFromTheInputs =
            method.IsGenericMethodDefinition || HasNoAnswerToStore(method.ReturnType) || parameters.Length != constructorInputs;

        if (makesNoInstanceFromTheInputs)
        {
            return false;
        }

        return parameters.All(parameter => IsReadableInput(parameter.ParameterType));
    }

    // The catalog's cases read into this strategy's parameter types, fresh on every call, so a strategy
    // that rewrites its input rewrites only its own copy; a design question's, as its call scripts.
    public CatalogColumns Columns()
    {
        if (_calls is not null)
        {
            return CatalogColumns.ForScripts(Solution);
        }

        return CatalogColumns.For(Solution, _inputTypes);
    }

    // The answer to the case at caseIndex of columns: the return value, or for a Try strategy the outputs
    // it found, LeetCode's [] when it found none; for a design question, the script's answer.
    public JsonNode AnswerTo(CatalogColumns columns, int caseIndex)
    {
        if (_calls is { } calls)
        {
            return ScriptAnswerTo(calls, columns, caseIndex);
        }

        var parameters = _method.GetParameters();
        var arguments = ArgumentsFor(parameters, columns, caseIndex);
        var returned = _method.Invoke(null, BindingFlags.DoNotWrapExceptions, binder: null, arguments, culture: null);
        var outputs = parameters.Where(parameter => parameter.IsOut).Select(parameter => arguments[parameter.Position]).ToList();

        if (_answersInPlace)
        {
            return AnswerNotation.Of(arguments[0]);
        }

        return NotationOf(returned, outputs);
    }

    // A design question's case replayed: its columns hold the script's calls and their arguments
    // (CatalogColumns.ForScripts).
    private static JsonNode ScriptAnswerTo(CatalogCalls calls, CatalogColumns columns, int caseIndex)
    {
        if (columns.ValueAt(0, caseIndex) is not string[] script || columns.ValueAt(1, caseIndex) is not JsonArray arguments)
        {
            throw new InvalidOperationException($"Case {caseIndex} holds no call script.");
        }

        return calls.AnswerTo(script, arguments);
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

    // A static local function compiles to an internal method of the class, named for the method around it
    // (<CountByLoop>g__Step|0_0), so it carries that method's By. The generator meets the same method in the
    // solutions assembly's metadata and leaves it out too.
    private static bool IsStrategy(MethodInfo method) =>
        (method.IsPublic || method.IsAssembly)
            && !method.IsSpecialName
            && !method.IsDefined(typeof(CompilerGeneratedAttribute))
            && IsStrategyNamed(method.Name);

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

    // A void method answers by mutating its input, and a ref return or a ref struct cannot sit in an array.
    private static bool HasNoAnswerToStore(Type returnType) =>
        returnType == typeof(void) || returnType.IsByRef || returnType.IsByRefLike;

    private static bool IsReadableInput(Type parameterType) => !parameterType.IsByRef && NotationReader.CanRead(parameterType);
}
