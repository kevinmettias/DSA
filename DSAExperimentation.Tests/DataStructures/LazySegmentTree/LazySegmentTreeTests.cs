using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class LazySegmentTreeTests
{
    [Fact]
    public void Query_RangeAddSum_FullRange_ReturnsTotalSumBeforeAnyUpdate()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3, 4, 5, 6, 7, 8]);

        Assert.Equal(36, tree.Query(0, 7));
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3, 4, 5, 6, 7, 8]);

        tree.UpdateRange(1, 4, 10);

        Assert.Equal(1, tree.Query(0, 0));
        Assert.Equal(54, tree.Query(1, 4));
        Assert.Equal(21, tree.Query(5, 7));
        Assert.Equal(76, tree.Query(0, 7));
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OverlappingUpdates_ComposeAdditively()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3, 4, 5, 6, 7, 8]);

        tree.UpdateRange(1, 4, 10);
        tree.UpdateRange(3, 6, 5);

        Assert.Equal(39, tree.Query(3, 4));
        Assert.Equal(23, tree.Query(5, 6));
        Assert.Equal(96, tree.Query(0, 7));
    }

    [Fact]
    public void Query_RangeAssignMax_ReturnsMaxInRangeBeforeAnyUpdate()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>([5, 3, 8, 1, 9, 2, 7, 4]);

        Assert.Equal(9, tree.Query(0, 7));
        Assert.Equal(8, tree.Query(0, 2));
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_AssignsValueAcrossWholeRange()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>([5, 3, 8, 1, 9, 2, 7, 4]);

        tree.UpdateRange(1, 4, 100);

        Assert.Equal(100, tree.Query(1, 4));
        Assert.Equal(7, tree.Query(5, 7));
        Assert.Equal(100, tree.Query(0, 7));
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_OverlappingAssignments_NewerAssignmentWins()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>([5, 3, 8, 1, 9, 2, 7, 4]);

        tree.UpdateRange(1, 4, 100);
        tree.UpdateRange(3, 6, 50);

        Assert.Equal(50, tree.Query(3, 6));
        Assert.Equal(100, tree.Query(1, 2));
        Assert.Equal(100, tree.Query(0, 7));
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3]);

        Assert.Equal(3, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, 3));
    }

    [Fact]
    public void UpdateRange_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.UpdateRange(2, 1, 5));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>([42]);

        Assert.Equal(42, tree.Query(0, 0));
    }

    // RangeAssignMaxOperation<int>'s TUpdate is int? with NoUpdate = null; passing that
    // sentinel straight through as an "update" would let ApplyToNode compose it away any
    // update already pending on a node before it's pushed to that node's children - guarded
    // at the public entry point rather than left as a silent-wrong-answer trap.
    [Fact]
    public void UpdateRange_RangeAssignMax_NoUpdateSentinel_ThrowsArgumentException()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>([5, 3, 8, 1]);

        Assert.Throws<ArgumentException>(() => tree.UpdateRange(0, 3, null));
    }
}
