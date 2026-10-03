using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeRightSideView;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.Tests.BinaryTreeRightSideView;

// Harness only. Both strategies are BinaryTreeRightSideViewSolution's - the
// level-grouped BFS and the right-first DFS; this file pins them to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape
// (BinaryTreeNode<int> is internal, so it cannot appear in a public TheoryData
// signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed partial class BinaryTreeRightSideViewSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void RightSideViewByDepthFirstRightFirst_LeetCodeExamples_ReturnsRightmostPerLevel(
        int?[] values, List<int> expected) =>
        Assert.Equal(expected, BinaryTreeRightSideViewSolution.RightSideViewByDepthFirstRightFirst(LeetCodeWireFormat.ToBinaryTree(values)));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they return the same view on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void RightSideView_AgreeOnEveryExample(int?[] values, List<int> expected) =>
        Assert.Equal(
            BinaryTreeRightSideViewSolution.RightSideViewByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(values)),
            BinaryTreeRightSideViewSolution.RightSideViewByDepthFirstRightFirst(LeetCodeWireFormat.ToBinaryTree(values)));
}
