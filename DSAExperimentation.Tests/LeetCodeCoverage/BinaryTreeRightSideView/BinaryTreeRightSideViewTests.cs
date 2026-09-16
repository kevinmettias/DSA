using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeRightSideView;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeRightSideView;

// Harness only. The level-grouped BFS is BinaryTreeRightSideViewSolution's -
// this file pins it to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed partial class BinaryTreeRightSideViewTests
{
    public static TheoryData<int?[], List<int>> Examples =>
        new()
        {
            { [1, 2, 3, null, 5, null, 4], [1, 3, 4] },
            { [1, null, 3], [1, 3] },
            { [], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RightSideViewByLevelGroupedTraversal_LeetCodeExamples_ReturnsRightmostPerLevel(
        int?[] values, List<int> expected) =>
        Assert.Equal(expected, BinaryTreeRightSideViewSolution.RightSideViewByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(values)));
}
