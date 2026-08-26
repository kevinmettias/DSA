using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeTests
{
    [Fact]
    public void Query_SumOperation_FullRange_ReturnsTotalSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(15, tree.Query(0, 4));
    }

    [Fact]
    public void Query_SumOperation_PartialRange_ReturnsRangeSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(9, tree.Query(1, 3));
    }

    [Fact]
    public void Query_SumOperation_SingleElementRange_ReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(3, tree.Query(2, 2));
    }

    [Fact]
    public void Update_SumOperation_ChangesOnlyQueriesCoveringThatIndex()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3, 4, 5]);

        tree.Update(2, 30);

        Assert.Equal(30, tree.Query(2, 2));
        Assert.Equal(3, tree.Query(0, 1));
        Assert.Equal(42, tree.Query(0, 4));
    }

    [Fact]
    public void Query_MinOperation_ReturnsSmallestInRange()
    {
        var tree = new SegmentTree<int, MinOperation<int>>([5, 3, 8, 1, 9]);

        Assert.Equal(1, tree.Query(0, 4));
        Assert.Equal(3, tree.Query(0, 1));
    }

    [Fact]
    public void Query_MaxOperation_ReturnsLargestInRange()
    {
        var tree = new SegmentTree<int, MaxOperation<int>>([5, 3, 8, 1, 9]);

        Assert.Equal(9, tree.Query(0, 4));
        Assert.Equal(5, tree.Query(0, 1));
    }

    [Fact]
    public void Update_MinOperation_LoweringAValueUpdatesRangeMinimum()
    {
        var tree = new SegmentTree<int, MinOperation<int>>([5, 3, 8, 1, 9]);

        tree.Update(2, -10);

        Assert.Equal(-10, tree.Query(0, 4));
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Equal(3, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, 3));
    }

    [Fact]
    public void Query_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(2, 1));
    }

    [Fact]
    public void Update_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([1, 2, 3]);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Update(3, 10));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([42]);

        Assert.Equal(42, tree.Query(0, 0));
    }
}
