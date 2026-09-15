using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.FlattenBinaryTreeToLinkedList;

// LeetCode 114. Flatten Binary Tree to Linked List: rewrite a tree in place so
// every node's Right pointer chains to the next node in preorder, with Left always
// null. Both strategies mutate the tree they are handed - there is no separate
// "build" input to hoist a prepared-input overload from.
internal static class FlattenBinaryTreeToLinkedListSolution
{
    // Textbook post-order splice: flatten each subtree first, then graft the
    // flattened left chain's tail onto the flattened right chain's head. Pure
    // pointer surgery, no repo traversal primitive.
    public static void FlattenByRecursiveSplice(BinaryTreeNode<int>? root) => Splice(root);

    // Compose this repo's own preorder walk (Algorithms.Traversal.TopDown, the same
    // engine AllRootToLeafPaths uses for LC 113) to collect nodes in preorder, then
    // relink the collected sequence into a right-only chain in a second pass. Visit
    // fires before Descend into either child, so the collected order is exactly
    // preorder regardless of subtree shape.
    public static void FlattenByTopDownPreorderRelink(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return;
        }

        var nodes = new List<BinaryTreeNode<int>>();

        TopDownTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            CollectPreorderHooks, List<BinaryTreeNode<int>>>(root, nodes);

        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].Left = null;
            var hasNextNode = i + 1 < nodes.Count;
            nodes[i].Right = hasNextNode ? NextNode(nodes, i) : null;
        }
    }

    // The node that follows the collected one in preorder, which the last node has
    // not got - hence the guard at the call site.
    private static BinaryTreeNode<int>? NextNode(List<BinaryTreeNode<int>> nodes, int index) => nodes[index + 1];

    private static BinaryTreeNode<int>? Splice(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return null;
        }

        var leftTail = Splice(node.Left);
        var rightTail = Splice(node.Right);

        if (leftTail is not null)
        {
            leftTail.Right = node.Right;
            node.Right = node.Left;
            node.Left = null;
        }

        return rightTail ?? leftTail ?? node;
    }

    private readonly struct CollectPreorderHooks
        : ITopDownHooks<BinaryTreeNode<int>, List<BinaryTreeNode<int>>>
    {
        public static void Visit(
            BinaryTreeNode<int> node, List<BinaryTreeNode<int>> state, int depth, NodePosition position)
            => state.Add(node);

        public static List<BinaryTreeNode<int>> Descend(
            BinaryTreeNode<int> parent, List<BinaryTreeNode<int>> parentState, BinaryTreeNode<int> child)
            => parentState;
    }
}
