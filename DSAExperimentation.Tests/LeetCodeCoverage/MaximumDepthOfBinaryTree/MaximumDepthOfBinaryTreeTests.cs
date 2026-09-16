using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.MaximumDepthOfBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumDepthOfBinaryTree;

// Harness only. Both strategies are MaximumDepthOfBinaryTreeSolution's - this
// file pins them to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed class MaximumDepthOfBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 9, 20, null, null, 15, 7], 3 },
            { [1, null, 2], 2 },
            { [], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByRecursion_LeetCodeExamples_ReturnsHeight(int?[] values, int expected) =>
        Assert.Equal(expected, MaximumDepthOfBinaryTreeSolution.MaxDepthByRecursion(LeetCodeWireFormat.ToBinaryTree(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByTreeMetrics_LeetCodeExamples_ReturnsHeight(int?[] values, int expected) =>
        Assert.Equal(expected, MaximumDepthOfBinaryTreeSolution.MaxDepthByTreeMetrics(LeetCodeWireFormat.ToBinaryTree(values)));
}
