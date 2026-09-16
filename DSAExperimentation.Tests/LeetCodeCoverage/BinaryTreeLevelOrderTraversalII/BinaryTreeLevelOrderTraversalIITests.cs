using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversalII;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversalII;

// Harness only. Both search strategies are
// BinaryTreeLevelOrderTraversalIISolution's - this file just pins them to
// LeetCode's published examples, given in LeetCode's own level-order array
// notation (null marking a missing child) since BinaryTreeNode is internal and
// cannot appear in a public TheoryData signature. Building the tree from that
// array is harness plumbing, not part of either strategy under test - both
// strategies already take a prebuilt root, exactly like LeetCode's own
// TreeNode-typed signature.
public sealed class BinaryTreeLevelOrderTraversalIITests
{
    public static TheoryData<int?[], List<List<int>>> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], [[15, 7], [9, 20], [3]] },
            { [1], [[1]] },
            { [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderBottomByQueue_LeetCodeExamples_ReturnsLevelsBottomUp(
        int?[] levelOrder, List<List<int>> expected) =>
        Assert.Equal(
            expected,
            BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByQueue(LeetCodeWireFormat.ToBinaryTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LevelOrderBottomByLevelGroupedTraversal_LeetCodeExamples_ReturnsLevelsBottomUp(
        int?[] levelOrder, List<List<int>> expected) =>
        Assert.Equal(
            expected,
            BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(levelOrder)));
}
