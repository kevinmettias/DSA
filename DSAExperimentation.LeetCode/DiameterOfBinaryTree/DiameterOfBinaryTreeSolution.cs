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

    // This repo's own bottom-up fold: DiameterAlgebra computes height and the best
    // diameter-through-this-node together in a single TreeFold pass.
    public static int DiameterByTreeMetricsFold(BinaryTreeNode<int> root)
        => TreeMetrics.Diameter<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);

    private static (int Height, int Diameter) DiameterVia(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (0, 0);
        }

        var childHeights = (Left: Height(node.Left), Right: Height(node.Right));
        var childDiameters = ChildDiameters(node);

        return CombineNodeMeasurements(childHeights, childDiameters);
    }

    // The two child diameters, each recomputed from scratch at every node visited -
    // that repeat is exactly what makes this arm O(n^2) overall.
    private static (int Left, int Right) ChildDiameters(BinaryTreeNode<int> node)
    {
        var (_, leftDiameter) = DiameterVia(node.Left);
        var (_, rightDiameter) = DiameterVia(node.Right);

        return (leftDiameter, rightDiameter);
    }

    // One node's measurement from its children's: the taller child carries the height,
    // and the widest of the path through this node and the two child diameters is the
    // best diameter seen at or below it.
    private static (int Height, int Diameter) CombineNodeMeasurements(
        (int Left, int Right) childHeights, (int Left, int Right) childDiameters)
    {
        var diameterThroughNode = childHeights.Left + childHeights.Right;
        var bestChildDiameter = Math.Max(childDiameters.Left, childDiameters.Right);
        var diameter = Math.Max(diameterThroughNode, bestChildDiameter);

        return (1 + Math.Max(childHeights.Left, childHeights.Right), diameter);
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : NodeHeight(node);

    // The node's own level on top of its taller subtree.
    private static int NodeHeight(BinaryTreeNode<int> node)
        => 1 + Math.Max(Height(node.Left), Height(node.Right));
}
