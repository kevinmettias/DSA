using System.Text;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SubtreeOfAnotherTree;

// LeetCode 572. Subtree of Another Tree: is there a node in root whose subtree
// is structurally identical to subRoot? The two strategies differ in whether
// that check runs directly against the tree shape at every node, or against a
// serialized preorder string.
internal static class SubtreeOfAnotherTreeSolution
{
    private const string NullMarker = "#null";

    // The textbook answer: try a node-by-node structural comparison at every
    // node of root, O(n*m). Deliberately written without this repo's
    // string-matching primitives - it is the arm the serialize-and-search
    // strategy below has to justify itself against.
    public static bool IsSubtreeByRecursiveCompareAtEveryNode(BinaryTreeNode<int>? root, BinaryTreeNode<int> subRoot)
        => root is not null &&
           (IsSame(root, subRoot) ||
            IsSubtreeByRecursiveCompareAtEveryNode(root.Left, subRoot) ||
            IsSubtreeByRecursiveCompareAtEveryNode(root.Right, subRoot));

    // Serializes both trees to a '#'-delimited preorder string (every token
    // prefixed with '#', so no two values can ever run together across a token
    // boundary) and checks subRoot's serialization is a substring of root's via
    // this repo's own KMP PrefixFunctionSearch, O(n+m).
    public static bool IsSubtreeBySerializeThenKmpSearch(BinaryTreeNode<int>? root, BinaryTreeNode<int> subRoot)
        => PrefixFunctionSearch.FindAll(Serialize(root), Serialize(subRoot)).Count > 0;

    private static string Serialize(BinaryTreeNode<int>? node)
    {
        var builder = new StringBuilder();
        AppendPreorder(node, builder);
        return builder.ToString();
    }

    private static bool IsSame(BinaryTreeNode<int>? p, BinaryTreeNode<int>? q)
        => p is null || q is null
            ? p is null && q is null
            : p.Value == q.Value && IsSame(p.Left, q.Left) && IsSame(p.Right, q.Right);

    private static void AppendPreorder(BinaryTreeNode<int>? node, StringBuilder builder)
    {
        if (node is null)
        {
            builder.Append(NullMarker);
            return;
        }

        builder.Append('#').Append(node.Value);
        AppendPreorder(node.Left, builder);
        AppendPreorder(node.Right, builder);
    }
}
