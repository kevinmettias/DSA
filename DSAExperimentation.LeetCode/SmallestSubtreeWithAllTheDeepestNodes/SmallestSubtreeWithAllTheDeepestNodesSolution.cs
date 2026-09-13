using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

// LeetCode 865. Smallest Subtree with all the Deepest Nodes: the deepest subtree
// root that still contains every node at the tree's maximum depth.
//
// Both strategies are the same bottom-up (depth, candidate) recurrence - a tie
// between the two children's depths promotes the answer to the current node, a
// single deeper child hands its own answer straight up. The baseline writes that
// recurrence out by hand over BinaryTreeNode<int>.Left/Right; the composed strategy
// states it once as DeepestSubtreeAlgebra and lets this repo's own generic TreeFold
// engine drive it - the same "hand-rolled vs. generic repo engine, same O(n) class,
// the difference is composability and dispatch" framing DiameterOfBinaryTreeSolution
// establishes for LC 543.
internal static class SmallestSubtreeWithAllTheDeepestNodesSolution
{
    // The textbook answer: plain recursion returning (depth, candidate) per node,
    // written with nothing but the node type's own child references - it is the arm
    // the composed strategy below has to justify itself against.
    public static BinaryTreeNode<int>? SubtreeWithAllDeepestByRecursion(BinaryTreeNode<int>? root)
        => DeepestSubtree(root).Node;

    private static (int Depth, BinaryTreeNode<int>? Node) DeepestSubtree(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (-1, null);
        }

        var left = DeepestSubtree(node.Left);
        var right = DeepestSubtree(node.Right);

        if (left.Depth > right.Depth)
        {
            return (left.Depth + 1, left.Node);
        }

        if (right.Depth > left.Depth)
        {
            return (right.Depth + 1, right.Node);
        }

        return (left.Depth + 1, node);
    }

    // This repo's own bottom-up fold: one TreeFold pass over BinaryTreeNode<int>
    // closed over DeepestSubtreeAlgebra, which carries the same recurrence as data.
    public static BinaryTreeNode<int>? SubtreeWithAllDeepestByTreeFold(BinaryTreeNode<int>? root)
        => TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            DeepestSubtreeAlgebra, (int Depth, BinaryTreeNode<int>? Node)>(root).Node;
}
