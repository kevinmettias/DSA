using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceInBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceInBST;

// Harness only. Both strategies are MinimumAbsoluteDifferenceInBSTSolution's - this
// file pins them to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape. BinaryTreeNode<int> is internal, so - as in
// RecoverBinarySearchTreeTests - it stays out of a public TheoryData signature and
// LeetCodeWireFormat.ToBinaryTree reconstructs it from that array.
public sealed class MinimumAbsoluteDifferenceInBSTTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            // [1,null,3,2] -> in-order 1,2,3 -> min diff 1
            { [1, null, 3, 2], 1 },
            // [4,2,6,1,3] -> in-order 1,2,3,4,6 -> min diff 1
            { [4, 2, 6, 1, 3], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMinimumDifferenceByRecursiveScan_LeetCodeExamples_ReturnsSmallestAdjacentGap(
        int?[] values, int expected)
    {
        var minimumDifference =
            MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByRecursiveScan(LeetCodeWireFormat.ToBinaryTree(values));

        Assert.Equal(expected, minimumDifference);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMinimumDifferenceByInOrderHooks_LeetCodeExamples_ReturnsSmallestAdjacentGap(
        int?[] values, int expected)
    {
        var minimumDifference =
            MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByInOrderHooks(LeetCodeWireFormat.ToBinaryTree(values));

        Assert.Equal(expected, minimumDifference);
    }
}
