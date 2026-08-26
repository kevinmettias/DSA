using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.Tests.DataStructures.RangeFenwickTree;

public sealed partial class RangeFenwickTreeTests
{
    [Fact]
    public void Constructor_SizeOnly_StartsAtZeroEverywhere()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(5);

        Assert.Equal(0, tree.Query(0, 4));
    }

    [Fact]
    public void RangeAdd_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(5);

        tree.RangeAdd(1, 3, 10);

        Assert.Equal(0, tree.Query(0, 0));
        Assert.Equal(30, tree.Query(1, 3));
        Assert.Equal(0, tree.Query(4, 4));
        Assert.Equal(30, tree.Query(0, 4));
    }

    [Fact]
    public void RangeAdd_OverlappingUpdates_ComposeAdditively()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(5);

        tree.RangeAdd(1, 3, 10);
        tree.RangeAdd(2, 4, 5);

        Assert.Equal(30, tree.Query(2, 3));
        Assert.Equal(5, tree.Query(4, 4));
        Assert.Equal(45, tree.Query(0, 4));
    }

    [Fact]
    public void RangeAdd_ExtendsToTheLastIndex_DoesNotThrowOnBoundaryCancellation()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(5);

        tree.RangeAdd(0, 4, 7);

        Assert.Equal(35, tree.Query(0, 4));
    }

    [Fact]
    public void Constructor_FromInitialValues_QueryMatchesRangeSum()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(15, tree.Query(0, 4));
        Assert.Equal(9, tree.Query(1, 3));
    }

    [Fact]
    public void RangeAdd_AfterConstructorFromInitialValues_ComposesWithInitialValues()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>([1, 2, 3, 4, 5]);

        tree.RangeAdd(1, 3, 10);

        Assert.Equal(1, tree.Query(0, 0));
        Assert.Equal(39, tree.Query(1, 3));
        Assert.Equal(5, tree.Query(4, 4));
        Assert.Equal(45, tree.Query(0, 4));
    }

    [Fact]
    public void PrefixQuery_ReturnsRunningSumUpToIndex()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>([1, 2, 3, 4, 5]);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(6, tree.PrefixQuery(2));
        Assert.Equal(15, tree.PrefixQuery(4));
    }

    [Fact]
    public void Query_SingleIndex_ActsAsAPointQuery()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>([1, 2, 3, 4, 5]);

        tree.RangeAdd(1, 3, 10);

        Assert.Equal(12, tree.Query(1, 1));
        Assert.Equal(14, tree.Query(3, 3));
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(7);

        Assert.Equal(7, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(3);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, 3));
    }

    [Fact]
    public void RangeAdd_RightOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(3);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.RangeAdd(0, 3, 5));
    }
}
