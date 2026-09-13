using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

// Per node, the deepest level reached anywhere in its subtree paired with the subtree
// root that already contains every one of THAT subtree's deepest nodes: a tie between
// children promotes the answer to this node, a single deeper child propagates its own
// answer unchanged. The same "(depth, candidate) fold state" shape DiameterAlgebra
// uses for LC 543, but tracking a node rather than a length.
//
// This algebra answers one LeetCode problem and nothing else - no TreeMetrics facade
// fits this exact combination - so it lives beside the solution rather than joining
// Algorithms/Metrics.
internal readonly struct DeepestSubtreeAlgebra
    : IFoldAlgebra<BinaryTreeNode<int>, (int Depth, BinaryTreeNode<int>? Node)>
{
    public static (int Depth, BinaryTreeNode<int>? Node) Empty => (-1, null);

    public static (int Depth, BinaryTreeNode<int>? Node) Combine(
        BinaryTreeNode<int> node, IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children)
    {
        if (children.Count == 0)
        {
            return (0, node);
        }

        var maxDepth = MaxChildDepth(children);
        var (deepest, tieCount) = DeepestTiedChild(children, maxDepth);

        return (maxDepth + 1, tieCount == 1 ? deepest : node);
    }

    private static int MaxChildDepth(IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children)
    {
        var maxDepth = 0;

        for (var i = 0; i < children.Count; i++)
        {
            if (children[i].Depth > maxDepth)
            {
                maxDepth = children[i].Depth;
            }
        }

        return maxDepth;
    }

    private static (BinaryTreeNode<int>? Deepest, int TieCount) DeepestTiedChild(
        IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children, int maxDepth)
    {
        BinaryTreeNode<int>? deepest = null;
        var tieCount = 0;

        for (var i = 0; i < children.Count; i++)
        {
            if (children[i].Depth == maxDepth)
            {
                tieCount++;
                deepest = children[i].Node;
            }
        }

        return (deepest, tieCount);
    }
}
