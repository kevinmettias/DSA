using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

internal static class BinaryTreeTrees
{
    //       4
    //      / \
    //     2   6
    //    / \   \
    //   1   3   7
    // Deliberately asymmetric (6 has a right child only, no left) so every fixture
    // consumer exercises both a two-child node and a right-only node, not just the
    // fully-balanced case.
    public static BinaryTreeNode<int> Sample()
    {
        var one = new BinaryTreeNode<int>(1);
        var three = new BinaryTreeNode<int>(3);
        var seven = new BinaryTreeNode<int>(7);
        var two = new BinaryTreeNode<int>(2) { Left = one, Right = three };
        var six = new BinaryTreeNode<int>(6) { Right = seven };
        return new BinaryTreeNode<int>(4) { Left = two, Right = six };
    }

    public static BinaryTreeNode<int> SingleNode() => new(1);

    // 2 -> Right = 3, no Left - isolates the exact shape that breaks a
    // compacted-IChildren-style "child 0 vs. the rest" in-order generalization.
    public static BinaryTreeNode<int> RightSkewedPair()
        => new BinaryTreeNode<int>(2) { Right = new BinaryTreeNode<int>(3) };

    public static BinaryTreeNode<int> LeftSkewedPair()
        => new BinaryTreeNode<int>(5) { Left = new BinaryTreeNode<int>(4) };
}
