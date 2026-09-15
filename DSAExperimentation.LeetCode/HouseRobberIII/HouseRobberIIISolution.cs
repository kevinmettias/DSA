using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.HouseRobberIII;

// LeetCode 337. House Robber III: houses form a binary tree; robbing two directly
// connected houses trips the alarm, so choose the subset with no parent-child edge
// between chosen houses that maximizes total value.
//
// Both strategies compute the same per-node (Robbed, NotRobbed) pair via a
// post-order walk - RobByRecursivePair does it by hand, RobByTreeFoldAlgebra closes
// this repo's generic TreeFold engine over RobFoldAlgebra, the exact same
// catamorphism shape TreeMetrics.Height uses for MaximumDepthOfBinaryTree, just with
// a richer per-node result than a single int.
internal static class HouseRobberIIISolution
{
    // The textbook answer: a hand-rolled post-order recursion returning the
    // (Robbed, NotRobbed) pair directly. Deliberately written without this repo's
    // fold engine - it is the arm TreeFold has to justify itself against.
    public static int RobByRecursivePair(BinaryTreeNode<int> root)
    {
        var (robbed, notRobbed) = Gain(root);
        return Math.Max(robbed, notRobbed);
    }

    // This repo's own TreeFold engine closed over RobFoldAlgebra - the same
    // composition CountWaysToBuildRoomsInAnAntColony uses over its own algebra.
    public static int RobByTreeFoldAlgebra(BinaryTreeNode<int> root)
    {
        var (robbed, notRobbed) = TreeFold.Fold<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RobFoldAlgebra,
            (int Robbed, int NotRobbed)>(root);

        return Math.Max(robbed, notRobbed);
    }

    private static (int Robbed, int NotRobbed) Gain(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (0, 0);
        }

        var left = Gain(node.Left);
        var right = Gain(node.Right);

        return (
            node.Value + left.NotRobbed + right.NotRobbed,
            Math.Max(left.Robbed, left.NotRobbed) + Math.Max(right.Robbed, right.NotRobbed));
    }
}
