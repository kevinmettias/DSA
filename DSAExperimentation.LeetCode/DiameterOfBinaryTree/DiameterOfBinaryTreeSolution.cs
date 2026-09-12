using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.DiameterOfBinaryTree;

// LeetCode 543. Diameter of Binary Tree: the longest path between any two nodes,
// measured in edges.
//
// The naive baseline recomputes the height of both subtrees from scratch at every
// node - O(n) work times n nodes, O(n^2) overall. The composed strategy is exactly
// this repo's own TreeMetrics.Diameter: DiameterAlgebra tracks, per node, the two
// tallest child heights (the best path THROUGH that node) alongside the best
// diameter seen in any subtree so far, via one bottom-up TreeFold pass - O(n).
internal static class DiameterOfBinaryTreeSolution
{
    // The textbook answer: plain recursion over BinaryTreeNode<int>, recomputing
    // height from scratch for every node visited. Deliberately written without
    // this repo's fold machinery - it is the arm the composed solution below has
    // to justify itself against.
    public static int DiameterByRecomputedHeightPerNode(BinaryTreeNode<int> root)
        => DiameterVia(root).Diameter;

    private static (int Height, int Diameter) DiameterVia(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (0, 0);
        }

        var leftHeight = Height(node.Left);
        var rightHeight = Height(node.Right);
        var (_, leftDiameter) = DiameterVia(node.Left);
        var (_, rightDiameter) = DiameterVia(node.Right);

        var diameterThroughNode = leftHeight + rightHeight;
        var bestChildDiameter = Math.Max(leftDiameter, rightDiameter);
        var diameter = Math.Max(diameterThroughNode, bestChildDiameter);

        return (1 + Math.Max(leftHeight, rightHeight), diameter);
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    // This repo's own bottom-up fold: DiameterAlgebra computes height and the best
    // diameter-through-this-node together in a single TreeFold pass.
    public static int DiameterByTreeMetricsFold(BinaryTreeNode<int> root)
        => TreeMetrics.Diameter<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);
}
