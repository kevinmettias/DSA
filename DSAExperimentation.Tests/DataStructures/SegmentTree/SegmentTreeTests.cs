using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeTests
{
    [Fact]
    public void Query_SumOperation_FullRange_ReturnsTotalSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.SumValues);

        var actual = tree.Query(0, Samples.LastIndex);

        Assert.Equal(Samples.SumTotal, actual);
    }

    [Fact]
    public void Query_SumOperation_PartialRange_ReturnsRangeSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.SumValues);

        var actual = tree.Query(1, Samples.PartialRangeRightIndex);

        Assert.Equal(Samples.PartialRangeSum, actual);
    }

    [Fact]
    public void Query_SumOperation_SingleElementRange_ReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.SumValues);

        var actual = tree.Query(Samples.MiddleIndex, Samples.MiddleIndex);

        Assert.Equal(Samples.ValueAtMiddleIndex, actual);
    }

    [Fact]
    public void Update_SumOperation_ChangesOnlyQueriesCoveringThatIndex()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.SumValues);
        tree.Update(Samples.MiddleIndex, Samples.UpdatedValue);

        var updatedElementQuery = tree.Query(Samples.MiddleIndex, Samples.MiddleIndex);

        Assert.Equal(Samples.UpdatedValue, updatedElementQuery);

        var firstTwoElementsQuery = tree.Query(0, 1);

        Assert.Equal(Samples.SumOfFirstTwoElements, firstTwoElementsQuery);

        var fullRangeQuery = tree.Query(0, Samples.LastIndex);

        Assert.Equal(Samples.SumAfterUpdate, fullRangeQuery);
    }

    [Fact]
    public void Query_MinOperation_ReturnsSmallestInRange()
    {
        var tree = new SegmentTree<int, MinOperation<int>>(Samples.MinMaxValues);

        var fullRangeMin = tree.Query(0, Samples.LastIndex);

        Assert.Equal(1, fullRangeMin);

        var firstTwoMin = tree.Query(0, 1);

        Assert.Equal(Samples.MinOfFirstTwoElements, firstTwoMin);
    }

    [Fact]
    public void Query_MaxOperation_ReturnsLargestInRange()
    {
        var tree = new SegmentTree<int, MaxOperation<int>>(Samples.MinMaxValues);

        var fullRangeMax = tree.Query(0, Samples.LastIndex);

        Assert.Equal(Samples.MaxOfSample, fullRangeMax);

        var firstTwoMax = tree.Query(0, 1);

        Assert.Equal(Samples.MaxOfFirstTwoElements, firstTwoMax);
    }

    [Fact]
    public void Update_MinOperation_LoweringAValueUpdatesRangeMinimum()
    {
        var tree = new SegmentTree<int, MinOperation<int>>(Samples.MinMaxValues);
        tree.Update(Samples.MiddleIndex, Samples.LoweredValue);

        var actual = tree.Query(0, Samples.LastIndex);

        Assert.Equal(Samples.LoweredValue, actual);
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.ThreeValues);

        Assert.Equal(Samples.ThreeValues.Length, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.ThreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, Samples.ThreeValues.Length));
    }

    [Fact]
    public void Query_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.ThreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => tree.Query(Samples.LastIndexInThreeValues, 1));
    }

    [Fact]
    public void Update_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(Samples.ThreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => tree.Update(Samples.ThreeValues.Length, Samples.AnyUpdateValue));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([Samples.SingleElementValue]);

        var actual = tree.Query(0, 0);

        Assert.Equal(Samples.SingleElementValue, actual);
    }

    // The sample inputs these tests build their trees from, and the values that follow
    // from each of them: one holder rather than sixteen constants and three arrays
    // loose in the test class, so the numbers a reader has to follow sit in one place
    // and the tests above read as the call sequences they are.
    private static class Samples
    {
        public static readonly int[] SumValues = [1, 2, 3, 4, 5];
        public static readonly int[] MinMaxValues = [5, 3, 8, 1, 9];
        public static readonly int[] ThreeValues = [1, 2, 3];

        public const int MiddleIndex = 2;
        public const int LastIndex = 4;
        public const int SumTotal = 15;
        public const int PartialRangeRightIndex = 3;
        public const int PartialRangeSum = 9;
        public const int ValueAtMiddleIndex = 3;
        public const int UpdatedValue = 30;
        public const int SumOfFirstTwoElements = 3;
        public const int SumAfterUpdate = 42;
        public const int MinOfFirstTwoElements = 3;
        public const int MaxOfSample = 9;
        public const int MaxOfFirstTwoElements = 5;
        public const int LoweredValue = -10;
        public const int LastIndexInThreeValues = 2;
        public const int AnyUpdateValue = 10;
        public const int SingleElementValue = 42;
    }
}
