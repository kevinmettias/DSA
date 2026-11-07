using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// Which of a strategy's overloads is LeetCode's shape (ARCHITECTURE 17.4), when a case could feed more than
// one, stated over compile-time types: the rule the harness's LeetCodeShape applies at run time, so the two
// bind the same overload. The LeetCode-shaped overload takes as many inputs as LeetCode's signature
// declares, of the types it declares. The declared types come from the question's snapshot file, which the
// benchmarks project hands this generator as an additional file.
internal static class LeetCodeShape
{
    private const string ArraySuffix = "[]";
    private const string ListPrefix = "list<";
    private const string ListSuffix = ">";
    private const string SignatureKey = "\"Signature\"";
    private const string ParametersKey = "\"Parameters\"";
    private const string ReturnTypeKey = "\"ReturnType\"";

    private static readonly Dictionary<string, SpecialType> Scalars = new(StringComparer.Ordinal)
    {
        ["integer"] = SpecialType.System_Int32,
        ["long"] = SpecialType.System_Int64,
        ["double"] = SpecialType.System_Double,
        ["boolean"] = SpecialType.System_Boolean,
        ["string"] = SpecialType.System_String,
        ["character"] = SpecialType.System_Char,
    };

    private static readonly Dictionary<string, string> Nodes = new(StringComparer.Ordinal)
    {
        ["TreeNode"] = "DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode`1",
        ["ListNode"] = "DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode`1",
    };

    // The sequences a strategy may read a declared array or list through.
    private static readonly string[] Sequences =
    [
        "System.Collections.Generic.IEnumerable`1",
        "System.Collections.Generic.IReadOnlyList`1",
        "System.Collections.Generic.IReadOnlyCollection`1",
        "System.Collections.Generic.List`1",
        "System.Collections.Generic.IList`1",
        "System.Collections.Generic.ICollection`1",
    ];

    private static readonly Regex DeclaredType = new("\"Type\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.Compiled);

    // The types a question's snapshot file declares for its function's parameters, in order: the Type of each
    // entry between its signature's Parameters and ReturnType. None for a design question or a file with no
    // signature.
    public static IReadOnlyList<string> DeclaredTypesIn(string questionJson)
    {
        var signature = questionJson.IndexOf(SignatureKey, StringComparison.Ordinal);
        var parameters = signature < 0 ? -1 : questionJson.IndexOf(ParametersKey, signature, StringComparison.Ordinal);
        var returnType = parameters < 0 ? -1 : questionJson.IndexOf(ReturnTypeKey, parameters, StringComparison.Ordinal);

        if (returnType < 0)
        {
            return [];
        }

        var span = questionJson.Substring(parameters, returnType - parameters);
        return [.. DeclaredType.Matches(span).Cast<Match>().Select(match => match.Groups[1].Value)];
    }

    // The overloads among these that take LeetCode's declared inputs: those of the declared count, then of
    // them those matching the most declared types. More than one when a tie leaves the choice open, and all
    // of them when none takes the declared count.
    public static IReadOnlyList<IMethodSymbol> Narrowed(IReadOnlyList<IMethodSymbol> overloads, IReadOnlyList<string> declared)
    {
        var ofTheCount = overloads.Where(overload => IsOfTheCount(overload, declared.Count)).ToList();

        if (ofTheCount.Count == 0)
        {
            return overloads;
        }

        var best = ofTheCount.Max(overload => MatchesOf(overload, declared));
        var leading = ofTheCount.Where(overload => MatchesOf(overload, declared) == best).ToList();

        // A tie is broken by reading each one-value role type as its value, as the harness's LeetCodeShape does.
        var bestRead = leading.Max(overload => ReadMatchesOf(overload, declared));
        return [.. leading.Where(overload => ReadMatchesOf(overload, declared) == bestRead)];
    }

    // Takes the declared count of inputs one to a parameter, or spread, a tuple or record parameter over as
    // many consecutive inputs as it has values, as CatalogColumns reads a case into it.
    private static bool IsOfTheCount(IMethodSymbol overload, int count)
    {
        var inputs = InputsOf(overload);

        return inputs.Count == count || inputs.Sum(NotationTypes.WidthOf) == count;
    }

    private static int MatchesOf(IMethodSymbol overload, IReadOnlyList<string> declared) =>
        InputsOf(overload).Zip(declared, IsDeclaredType).Count(isMatch => isMatch);

    private static int ReadMatchesOf(IMethodSymbol overload, IReadOnlyList<string> declared) =>
        InputsOf(overload).Zip(declared, (type, name) => IsDeclaredType(NotationTypes.RoleValueOf(type) ?? type, name)).Count(isMatch => isMatch);

    // Whether a parameter type is the type LeetCode declares, read as this repository reads its inputs.
    public static bool IsDeclaredType(ITypeSymbol type, string declared)
    {
        // A type the notation reads into no type of this repository's own - LeetCode's NestedInteger - is read as
        // written, into a JsonNode.
        if (NotationTypes.IsJsonNode(type))
        {
            return !IsReadableDeclared(declared);
        }

        if (Scalars.TryGetValue(declared, out var scalar))
        {
            return type.SpecialType == scalar;
        }

        if (Nodes.TryGetValue(declared, out var node))
        {
            return type is INamedTypeSymbol { IsGenericType: true } named && MetadataNameOf(named.OriginalDefinition) == node;
        }

        return ElementTypeOf(declared) is { } element && SequenceItemOf(type) is { } item && IsDeclaredType(item, element);
    }

    // T for a declared T[] or list<T>; null for any other declared type.
    private static string? ElementTypeOf(string declared)
    {
        if (declared.EndsWith(ArraySuffix, StringComparison.Ordinal))
        {
            return declared.Substring(0, declared.Length - ArraySuffix.Length);
        }

        if (declared.StartsWith(ListPrefix, StringComparison.Ordinal) && declared.EndsWith(ListSuffix, StringComparison.Ordinal))
        {
            return declared.Substring(ListPrefix.Length, declared.Length - ListPrefix.Length - ListSuffix.Length);
        }

        return null;
    }

    // The item type of a one-dimensional array or of one of the sequences a strategy reads through.
    private static ITypeSymbol? SequenceItemOf(ITypeSymbol type) => type switch
    {
        IArrayTypeSymbol { Rank: 1 } array => array.ElementType,
        INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } named when Sequences.Contains(MetadataNameOf(named.OriginalDefinition)) => named.TypeArguments[0],
        _ => null,
    };

    // Whether the notation reads LeetCode's declared type into a type of this repository's own.
    private static bool IsReadableDeclared(string declared)
    {
        if (Scalars.ContainsKey(declared) || Nodes.ContainsKey(declared))
        {
            return true;
        }

        return ElementTypeOf(declared) is { } element && IsReadableDeclared(element);
    }

    private static List<ITypeSymbol> InputsOf(IMethodSymbol overload) =>
        [.. overload.Parameters.Where(parameter => parameter.RefKind != RefKind.Out).Select(parameter => parameter.Type)];

    private static string MetadataNameOf(INamedTypeSymbol type) =>
        $"{type.ContainingNamespace.ToDisplayString()}.{type.MetadataName}";
}
