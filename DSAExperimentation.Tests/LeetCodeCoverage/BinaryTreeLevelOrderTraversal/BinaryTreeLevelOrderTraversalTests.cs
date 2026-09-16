using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversal;

// Harness only. Both strategies are BinaryTreeLevelOrderTraversalSolution's -
// this file pins them to LeetCode's published examples, given in LeetCode's
// own level-order-with-null array shape (BinaryTreeNode<int> is internal, so
// it cannot appear in a public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs
// it).
public sealed class BinaryTreeLevelOrderTraversalTests
{
    public static TheoryData<int?[], List<List<int>>> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], [[3], [9, 20], [15, 7]] },
            { [1], [[1]] },
            { [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderByQueueLevels_LeetCodeExamples_ReturnsLevelsTopDown(
        int?[] values, List<List<int>> expected) =>
        Assert.Equal(expected, BinaryTreeLevelOrderTraversalSolution.LevelOrderByQueueLevels(LeetCodeWireFormat.ToBinaryTree(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderByLevelGroupedTraversal_LeetCodeExamples_ReturnsLevelsTopDown(
        int?[] values, List<List<int>> expected) =>
        Assert.Equal(
            expected,
            BinaryTreeLevelOrderTraversalSolution.LevelOrderByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(values)));
}
