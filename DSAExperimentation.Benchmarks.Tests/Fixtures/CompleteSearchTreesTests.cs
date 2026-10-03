using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CompleteSearchTrees (ARCHITECTURE 17.7). Its comment promises two things: the tree has
// BinaryTrees.Complete's shape, and its values are their in-order ranks, which is what makes it a binary search
// tree. The shape is compared node position by node position against BinaryTrees.Balanced, which lays out the
// same complete tree; the ranks are read back by an in-order walk, which for a BST must ascend through 0..n-1.
public sealed partial class CompleteSearchTreesTests
{
    public static TheoryData<int> NodeCounts => [1, 2, 10, 200];

    [Theory]
    [MemberData(nameof(NodeCounts))]
    public void InOrderRanked_InOrderWalk_AscendsThroughEveryRank(int nodeCount) =>
        Assert.Equal(Enumerable.Range(0, nodeCount), InOrderValues(CompleteSearchTrees.InOrderRanked(nodeCount)));

    [Theory]
    [MemberData(nameof(NodeCounts))]
    public void InOrderRanked_Shape_IsTheCompleteTreeBalancedLaysOut(int nodeCount) =>
        Assert.Equal(
            OccupiedPositions(BinaryTrees.Balanced(nodeCount)),
            OccupiedPositions(CompleteSearchTrees.InOrderRanked(nodeCount)));

    // Which level-order positions hold a node, in LeetCode's array notation, values aside.
    private static bool[] OccupiedPositions(BinaryTreeNode<int> root) =>
        [.. LeetCodeWireFormat.FromBinaryTree(root).Select(value => value is not null)];

    private static List<int> InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendInOrder(root, values);
        return values;
    }

    private static void AppendInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(node.Left, values);
        values.Add(node.Value);
        AppendInOrder(node.Right, values);
    }
}
