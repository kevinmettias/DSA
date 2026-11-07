using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness;

// Which of a strategy's overloads is LeetCode's shape (ARCHITECTURE 17.4), when a case could feed more than
// one: a hoisted overload may take a BCL type the notation reads too - (int Start, int End)[] beside
// int[][], List<int>[] beside an edge list - which 17.4 allows, since no C# call can confuse them. The
// LeetCode-shaped overload takes as many inputs as LeetCode's signature declares, of the types it declares:
// a declared integer[] is an int[], or a sequence of int a strategy reads through an interface, and never a
// tuple. CatalogBenchmarkGenerator narrows by the same rule over compile-time types.
internal static class LeetCodeShape
{
    private const string ArraySuffix = "[]";
    private const string ListPrefix = "list<";
    private const string ListSuffix = ">";

    private static readonly Dictionary<string, Type> Scalars = new(StringComparer.Ordinal)
    {
        ["integer"] = typeof(int),
        ["long"] = typeof(long),
        ["double"] = typeof(double),
        ["boolean"] = typeof(bool),
        ["string"] = typeof(string),
        ["character"] = typeof(char),
        ["TreeNode"] = typeof(BinaryTreeNode<int>),
        ["ListNode"] = typeof(SinglyLinkedListNode<int>),
    };

    // The sequences a strategy may read a declared array or list through.
    private static readonly Type[] Sequences =
    [
        typeof(IEnumerable<>), typeof(IReadOnlyList<>), typeof(IReadOnlyCollection<>), typeof(List<>), typeof(IList<>), typeof(ICollection<>),
    ];

    // The overloads among these that take LeetCode's declared inputs: those of the declared count, then of
    // them those matching the most declared types. More than one when a tie leaves the choice open, and all
    // of them when none takes the declared count, as for a question with an input only the judge reads.
    public static IReadOnlyList<MethodInfo> Narrowed(IReadOnlyList<MethodInfo> overloads, IReadOnlyList<string> declared)
    {
        var ofTheCount = overloads.Where(overload => IsOfTheCount(overload, declared.Count)).ToList();

        if (ofTheCount.Count == 0)
        {
            return overloads;
        }

        var best = ofTheCount.Max(overload => MatchesOf(overload, declared));
        var leading = ofTheCount.Where(overload => MatchesOf(overload, declared) == best).ToList();

        // A tie between overloads that match alike is broken by reading each one-value role type as its value,
        // as the notation reads it: Shortest Matching Substring's pattern as a WildcardPattern(string) matches
        // LeetCode's string, where the same pattern split into a three-part record does not.
        var bestRead = leading.Max(overload => ReadMatchesOf(overload, declared));
        return [.. leading.Where(overload => ReadMatchesOf(overload, declared) == bestRead)];
    }

    // Takes the declared count of inputs one to a parameter, or spread, a tuple or record parameter over as
    // many consecutive inputs as it has values, as CatalogColumns reads a case into it.
    private static bool IsOfTheCount(MethodInfo overload, int count)
    {
        var inputs = InputsOf(overload);

        return inputs.Count == count || inputs.Sum(NotationReader.WidthOf) == count;
    }

    private static int MatchesOf(MethodInfo overload, IReadOnlyList<string> declared) =>
        InputsOf(overload).Zip(declared).Count(pair => IsDeclaredType(pair.First, pair.Second));

    private static int ReadMatchesOf(MethodInfo overload, IReadOnlyList<string> declared) =>
        InputsOf(overload).Zip(declared).Count(pair => IsDeclaredType(NotationReader.RoleValueOf(pair.First) ?? pair.First, pair.Second));

    // Whether a parameter type is the type LeetCode declares, read as this repository reads its inputs.
    public static bool IsDeclaredType(Type type, string declared)
    {
        // A type the notation reads into no type of this repository's own - LeetCode's NestedInteger - is read as
        // written, into a JsonNode.
        if (typeof(JsonNode).IsAssignableFrom(type))
        {
            return !IsReadableDeclared(declared);
        }

        if (Scalars.TryGetValue(declared, out var scalar))
        {
            return type == scalar;
        }

        return ElementTypeOf(declared) is { } element && SequenceItemOf(type) is { } item && IsDeclaredType(item, element);
    }

    // T for a declared T[] or list<T>; null for any other declared type.
    private static string? ElementTypeOf(string declared)
    {
        if (declared.EndsWith(ArraySuffix, StringComparison.Ordinal))
        {
            return declared[..^ArraySuffix.Length];
        }

        if (declared.StartsWith(ListPrefix, StringComparison.Ordinal) && declared.EndsWith(ListSuffix, StringComparison.Ordinal))
        {
            return declared[ListPrefix.Length..^ListSuffix.Length];
        }

        return null;
    }

    // The item type of a one-dimensional array or of one of the sequences a strategy reads through.
    private static Type? SequenceItemOf(Type type)
    {
        if (type.IsArray && type.GetArrayRank() == 1)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType && Sequences.Contains(type.GetGenericTypeDefinition()))
        {
            return type.GetGenericArguments()[0];
        }

        return null;
    }

    // Whether the notation reads LeetCode's declared type into a type of this repository's own.
    private static bool IsReadableDeclared(string declared)
    {
        if (Scalars.ContainsKey(declared))
        {
            return true;
        }

        return ElementTypeOf(declared) is { } element && IsReadableDeclared(element);
    }

    private static List<Type> InputsOf(MethodInfo overload) =>
        [.. overload.GetParameters().Where(parameter => !parameter.IsOut).Select(parameter => parameter.ParameterType)];
}
