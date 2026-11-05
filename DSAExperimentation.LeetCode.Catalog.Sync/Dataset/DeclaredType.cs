using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// Whether an input's value is of the type its parameter declares in LeetCode's vocabulary. The
// dataset's inputs were generated, and a few break their own declared type: a 1.5 in an integer
// tree, a nested [] in an integer[][], a bare number in a string[]. LeetCode's judge would never
// pose such an input. A type this does not name, such as NestedInteger, is taken as declared,
// since nothing here can say otherwise.
internal static class DeclaredType
{
    private const string Integer = "integer";
    private const string Long = "long";
    private const string ArraySuffix = "[]";
    private const string ListPrefix = "list<";
    private const string ListSuffix = ">";
    private const string TreeNode = "TreeNode";
    private const string ListNode = "ListNode";

    private static readonly Dictionary<string, Func<JsonNode, bool>> Scalars = new(StringComparer.Ordinal)
    {
        [Integer] = value => value is JsonValue number && number.TryGetValue<int>(out _),
        [Long] = value => value is JsonValue number && number.TryGetValue<long>(out _),
        ["double"] = value => value is JsonValue number && number.TryGetValue<double>(out _),
        ["boolean"] = value => value is JsonValue flag && flag.TryGetValue<bool>(out _),
        ["string"] = value => value is JsonValue text && text.TryGetValue<string>(out _),
        ["character"] = value => value is JsonValue text && text.TryGetValue<string>(out var letter) && letter.Length == 1,
    };

    // Whether the type holds whole numbers only, which LeetCode writes with no fraction.
    public static bool IsWholeNumber(string? type) => type is Integer or Long;

    public static bool IsOf(JsonNode? value, string type)
    {
        if (Scalars.TryGetValue(type, out var isScalar))
        {
            return value is not null && isScalar(value);
        }

        return type switch
        {
            TreeNode => value is JsonArray levels && levels.All(node => node is null || IsOf(node, Integer)),
            ListNode => IsSequenceOf(value, Integer),
            _ => ElementTypeOf(type) is not { } element || IsSequenceOf(value, element),
        };
    }

    private static bool IsSequenceOf(JsonNode? value, string element) =>
        value is JsonArray items && items.All(item => IsOf(item, element));

    // T for T[] and for list<T>; null for any other type.
    private static string? ElementTypeOf(string type)
    {
        if (type.EndsWith(ArraySuffix, StringComparison.Ordinal))
        {
            return type[..^ArraySuffix.Length];
        }

        if (type.StartsWith(ListPrefix, StringComparison.Ordinal) && type.EndsWith(ListSuffix, StringComparison.Ordinal))
        {
            return type[ListPrefix.Length..^ListSuffix.Length];
        }

        return null;
    }
}
