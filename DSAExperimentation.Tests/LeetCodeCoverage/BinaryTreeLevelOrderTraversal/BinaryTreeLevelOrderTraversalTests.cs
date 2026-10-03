using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversal;

// Harness only. Both strategies are BinaryTreeLevelOrderTraversalSolution's -
// this file pins them to LeetCode's published examples, given in LeetCode's
// own level-order-with-null array shape (BinaryTreeNode<int> is internal, so
// it cannot appear in a public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs
// it).
public sealed partial class BinaryTreeLevelOrderTraversalTests
{
    public static TheoryData<int?[], List<List<int>>> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], [[3], [9, 20], [15, 7]] },
            { [1], [[1]] },
            { [], [] },

            // A left spine: every level holds one node, so a level boundary
            // misplaced by one would merge two levels.
            { [1, 2, null, 3], [[1], [2], [3]] },
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
