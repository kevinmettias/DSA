namespace DSAExperimentation.LeetCode.Catalog.Sync;

// The names in LeetCode's type vocabulary that the sync tool branches on.
internal static class LeetCodeTypeNames
{
    // What a function that answers by changing its input returns.
    public const string Void = "void";

    // The node types whose empty value LeetCode writes as "[]" and Python's reference solutions
    // return as None: an empty tree or list is a legitimate answer for these and nothing else.
    public static readonly IReadOnlySet<string> NodeTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "TreeNode",
        "ListNode",
        "Node",
    };
}
