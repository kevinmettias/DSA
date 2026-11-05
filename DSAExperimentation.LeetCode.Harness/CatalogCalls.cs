using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Harness;

// How one strategy of a design question answers LeetCode's call scripts (ARCHITECTURE 17.13). The strategy is
// a factory: it takes the constructor's inputs and makes the object the script calls. Each of LeetCode's
// methods is answered by the method of the same name, ignoring case, that takes the call's arguments: a
// static method of the question's Calls type, which takes the object first, or else the object's own.
// CatalogBenchmarkGenerator binds the same methods at build time, so a generated arm makes the calls this
// replay makes.
internal sealed class CatalogCalls
{
    private const BindingFlags Statics = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags Instances = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    // An argument LeetCode lists as null, as a missing tree child is written.
    private const string JsonNull = "null";

    // What separates the words of a few of LeetCode's method names, as in get_root.
    private const string WordBreak = "_";

    private readonly MethodInfo _factory;
    private readonly Dictionary<string, MethodInfo> _answererByCall;

    private CatalogCalls(MethodInfo factory, Dictionary<string, MethodInfo> answererByCall)
    {
        _factory = factory;
        _answererByCall = answererByCall;
    }

    // The method answering each of the question's calls on the object the factory makes.
    public static CatalogCalls For(MethodInfo factory, Type? calls, IReadOnlyList<LeetCodeMethod> methods)
    {
        var answererByCall = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);

        foreach (var method in methods)
        {
            answererByCall[method.Name] = AnswererOf(factory.ReturnType, calls, method)
                ?? throw new InvalidOperationException(
                    $"{factory.DeclaringType?.Name}.{factory.Name} makes a {factory.ReturnType.Name} with no one method answering {method.Name}, and the question names no Calls method for it.");
        }

        return new CatalogCalls(factory, answererByCall);
    }

    // The script's answer as LeetCode writes it: null for the constructor, then each call's return value,
    // or null for a call that returns nothing.
    public JsonArray AnswerTo(IReadOnlyList<string> calls, JsonArray arguments)
    {
        var target = Invoke(_factory, null, ArgumentsOf(_factory.GetParameters(), arguments[0]));
        var answers = new JsonArray { null };

        for (var call = 1; call < calls.Count; call++)
        {
            answers.Add(AnswerOf(_answererByCall[calls[call]], target, arguments[call]));
        }

        return answers;
    }

    private static JsonNode? AnswerOf(MethodInfo answerer, object? target, JsonNode? arguments)
    {
        var returned = CallOf(answerer, target, arguments);

        if (answerer.ReturnType == typeof(void))
        {
            return null;
        }

        return AnswerNotation.Of(returned);
    }

    // A Calls method takes the object before the call's arguments; the object's own method is called on it.
    private static object? CallOf(MethodInfo answerer, object? target, JsonNode? arguments)
    {
        var parameters = answerer.GetParameters();

        if (!answerer.IsStatic)
        {
            return Invoke(answerer, target, ArgumentsOf(parameters, arguments));
        }

        object?[] withTarget = [target, .. ArgumentsOf(parameters[1..], arguments)];
        return Invoke(answerer, null, withTarget);
    }

    private static object? Invoke(MethodInfo method, object? target, object?[] arguments) =>
        method.Invoke(target, BindingFlags.DoNotWrapExceptions, binder: null, arguments, culture: null);

    // A call's arguments, as LeetCode lists them, read into the parameters that take them.
    private static object?[] ArgumentsOf(ParameterInfo[] parameters, JsonNode? arguments) =>
        [.. parameters.Select((parameter, position) => NotationReader.Read(ArgumentTextOf(arguments, position), parameter.ParameterType))];

    // One argument of a call, from the array LeetCode lists the call's arguments in, as notation to read.
    public static string ArgumentTextOf(JsonNode? arguments, int position) =>
        arguments?.AsArray()[position]?.ToJsonString() ?? JsonNull;

    // The Calls type's method for the call, or else the object's own: of the call's name, ignoring case, and
    // taking its arguments. Null when neither has exactly one.
    private static MethodInfo? AnswererOf(Type made, Type? calls, LeetCodeMethod method)
    {
        var count = method.Parameters.Count;
        var stated = calls?.GetMethods(Statics).Where(candidate => IsNamedFor(candidate, method) && TakesTheObjectAndArguments(candidate, made, count)).ToList() ?? [];

        if (stated.Count == 1)
        {
            return stated[0];
        }

        var own = InstanceMethodsOf(made).Where(candidate => IsNamedFor(candidate, method) && candidate.GetParameters().Length == count).ToList();

        if (own.Count == 1)
        {
            return own[0];
        }

        return null;
    }

    // Named for the call ignoring case, and the underscores LeetCode writes in a few names (get_root).
    private static bool IsNamedFor(MethodInfo candidate, LeetCodeMethod method) =>
        string.Equals(candidate.Name, method.Name.Replace(WordBreak, string.Empty), StringComparison.OrdinalIgnoreCase);

    private static bool TakesTheObjectAndArguments(MethodInfo candidate, Type made, int count)
    {
        var parameters = candidate.GetParameters();

        return parameters.Length == count + 1 && parameters[0].ParameterType.IsAssignableFrom(made);
    }

    // The public or internal instance methods of the object's type, as CatalogBenchmarkGenerator reads them:
    // an interface's with those of the interfaces it extends, a class's with those it inherits.
    private static IEnumerable<MethodInfo> InstanceMethodsOf(Type made) =>
        TypesDeclaringMethodsOf(made)
            .SelectMany(type => type.GetMethods(Instances))
            .Where(method => (method.IsPublic || method.IsAssembly) && method.DeclaringType != typeof(object))
            .Distinct();

    // A class implements its interfaces' methods itself, so only an interface reads the ones it extends.
    private static IEnumerable<Type> TypesDeclaringMethodsOf(Type made)
    {
        if (!made.IsInterface)
        {
            return [made];
        }

        return made.GetInterfaces().Prepend(made);
    }
}
