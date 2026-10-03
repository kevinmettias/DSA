using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.KthSmallestElementInABST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestElementInABST;

// Harness only: both strategies live in KthSmallestElementInABSTSolution and are
// asserted against LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape. BinaryTreeNode<int> is internal, so it stays
// out of the public TheoryData signature and LeetCodeWireFormat.ToBinaryTree builds
// the tree from that shape inside each test.
public sealed partial class KthSmallestElementInABSTTests
{
    public static TheoryData<RankExample> Examples =>
        new()
        {
            // [3,1,4,null,2], k = 1 -> 1
            { new RankExample([3, 1, 4, null, 2], Rank: 1, Expected: 1) },

            // [5,3,6,2,4,null,null,1], k = 3 -> 3
            { new RankExample([5, 3, 6, 2, 4, null, null, 1], Rank: 3, Expected: 3) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestByRecursiveWalk_LeetCodeExamples_ReturnsKthInOrderValue(RankExample example)
    {
        var actual = KthSmallestElementInABSTSolution.KthSmallestByRecursiveWalk(
            LeetCodeWireFormat.ToBinaryTree(example.LevelOrder), example.Rank);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestByInOrderTraversal_LeetCodeExamples_ReturnsKthInOrderValue(RankExample example)
    {
        var actual = KthSmallestElementInABSTSolution.KthSmallestByInOrderTraversal(
            LeetCodeWireFormat.ToBinaryTree(example.LevelOrder), example.Rank);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the tree in LeetCode's level-order shape, the rank asked for,
    // and the value at that rank. Rank and Expected are both ints, so each is named rather
    // than left as an interchangeable position.
    public readonly record struct RankExample(int?[] LevelOrder, int Rank, int Expected);
}
