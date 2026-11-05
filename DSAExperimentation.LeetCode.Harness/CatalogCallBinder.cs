using System.Reflection;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Harness;

// Which method answers each of a design question's calls on the instance a strategy's factory makes
// (ARCHITECTURE 17.13): the method of the call's name, ignoring case and LeetCode's underscores, that takes
// the call's arguments - a static method of the question's Calls type, which takes the instance first, or
// else the instance's own. CatalogBenchmarkGenerator's ScriptShape binds the same methods at build time.
internal static class CatalogCallBinder
{
    private const BindingFlags Statics = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags Instances = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    // What separates the words of a few of LeetCode's method names, as in get_root.
    private const string WordBreak = "_";

    // The replay of the question's scripts on the instance the factory makes, each call bound to its method.
    public static CatalogCalls Bind(MethodInfo factory, Type? calls, IReadOnlyList<LeetCodeMethod> methods)
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

    // The Calls type's method for the call, or else the instance's own: of the call's name, ignoring case, and
    // taking its arguments. Null when neither has exactly one.
    private static MethodInfo? AnswererOf(Type made, Type? calls, LeetCodeMethod method)
    {
        var count = method.Parameters.Count;
        var stated = calls?.GetMethods(Statics).Where(candidate => IsNamedFor(candidate, method) && IsStatedFor(candidate, made, count)).ToList() ?? [];

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
    private static bool IsNamedFor(MethodInfo candidate, LeetCodeMethod method)
    {
        var name = method.Name.Replace(WordBreak, string.Empty);

        return string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase);
    }

    // Whether a Calls method takes the instance the factory makes, then the call's arguments.
    private static bool IsStatedFor(MethodInfo candidate, Type made, int count)
    {
        var parameters = candidate.GetParameters();

        return parameters.Length == count + 1 && parameters[0].ParameterType.IsAssignableFrom(made);
    }

    // The public or internal instance methods of the made type, as CatalogBenchmarkGenerator reads them: an
    // interface's with those of the interfaces it extends, a class's with those it inherits.
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
