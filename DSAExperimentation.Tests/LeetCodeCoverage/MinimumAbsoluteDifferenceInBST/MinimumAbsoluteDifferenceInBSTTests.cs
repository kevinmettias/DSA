using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceInBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceInBST;

// Harness only. Both strategies are MinimumAbsoluteDifferenceInBSTSolution's - this
// file pins them to LeetCode's published examples. BinaryTreeNode<int> is internal,
// so - as in RecoverBinarySearchTreeTests - it stays out of a public
// TheoryData/[Theory] signature; each example is a private factory instead.
public sealed class MinimumAbsoluteDifferenceInBSTTests
{
    [Fact]
    public void GetMinimumDifferenceByRecursiveScan_ThreeNodeTree_ReturnsOne() =>
        Assert.Equal(1, MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByRecursiveScan(ThreeNodeTree()));

    [Fact]
    public void GetMinimumDifferenceByInOrderHooks_ThreeNodeTree_ReturnsOne() =>
        Assert.Equal(1, MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByInOrderHooks(ThreeNodeTree()));

    [Fact]
    public void GetMinimumDifferenceByRecursiveScan_LargerTree_ReturnsSmallestAdjacentGap() =>
        Assert.Equal(1, MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByRecursiveScan(LargerTree()));

    [Fact]
    public void GetMinimumDifferenceByInOrderHooks_LargerTree_ReturnsSmallestAdjacentGap() =>
        Assert.Equal(1, MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByInOrderHooks(LargerTree()));

    // [1,null,3,2] -> in-order 1,2,3 -> min diff 1
    private static BinaryTreeNode<int> ThreeNodeTree() =>
        new(1) { Right = new(3) { Left = new(2) } };

    // [4,2,6,1,3] -> in-order 1,2,3,4,6 -> min diff 1
    private static BinaryTreeNode<int> LargerTree() =>
        new(4)
        {
            Left = new(2) { Left = new(1), Right = new(3) },
            Right = new(6),
        };
}
