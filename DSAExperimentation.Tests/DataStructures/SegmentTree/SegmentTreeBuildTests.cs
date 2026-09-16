using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeBuildTests
{
    private const int LeafCount = 4;
    private const int RootNode = 0;
    private const int ArenaFill = 99;

    // For four leaves the partition over SegmentTreeIndex's layout is root 0, children 1 and 2,
    // grandchildren 3..6 - the only nodes Fill visits, which makes 7 the first node it never
    // touches.
    private const int FirstUnvisitedNode = 7;

    // One case per node Fill visits, in SegmentTreeIndex's order: the root's total, each
    // child's sum of the two leaves beneath it, then the four leaves themselves.
    public static TheoryData<int, int> NodeToExpectedCombinedValue =>
        new() { { RootNode, 10 }, { 1, 3 }, { 2, 7 }, { 3, 1 }, { 4, 2 }, { 5, 3 }, { 6, 4 } };

    [Fact]
    public void Fill_SingleLeaf_StoresTheSourceElementAtTheRoot()
    {
        var values = new SegmentTreeArray<int>(1);

        SegmentTreeBuild.Fill<int, SumOperation<int>>(values, new SegmentRange(RootNode, 0, 0), [7]);

        Assert.Equal(7, values.Get(RootNode));
    }

    [Theory]
    [MemberData(nameof(NodeToExpectedCombinedValue))]
    public void Fill_FourLeaves_StoresEachLeafAndCombinesEachInternalNode(int node, int expected)
    {
        var values = new SegmentTreeArray<int>(LeafCount);

        SegmentTreeBuild.Fill<int, SumOperation<int>>(values, new SegmentRange(RootNode, 0, LeafCount - 1), [1, 2, 3, 4]);

        Assert.Equal(expected, values.Get(node));
    }

    [Fact]
    public void Fill_LeavesNodesOutsideThePartitionAtTheArenasOwnFill()
    {
        var values = new SegmentTreeArray<int>(LeafCount, ArenaFill);

        SegmentTreeBuild.Fill<int, SumOperation<int>>(values, new SegmentRange(RootNode, 0, LeafCount - 1), [1, 2, 3, 4]);

        Assert.Equal(ArenaFill, values.Get(FirstUnvisitedNode));
    }
}
