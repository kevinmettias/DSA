using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeTests
{
    private const int MiddleIndex = 2;
    private const int LastIndex = 4;
    private const int SumOfSample = 15;
    private const int PartialRangeRightIndex = 3;
    private const int PartialRangeSum = 9;
    private const int ValueAtMiddleIndex = 3;
    private const int UpdatedValue = 30;
    private const int SumOfFirstTwoElements = 3;
    private const int SumAfterUpdate = 42;
    private const int MinOfFirstTwoElements = 3;
    private const int MaxOfSample = 9;
    private const int MaxOfFirstTwoElements = 5;
    private const int LoweredValue = -10;
    private const int LastIndexOfThreeElementArray = 2;
    private const int AnyUpdateValue = 10;
    private const int SingleElementValue = 42;

    private static readonly int[] SumSampleValues = [1, 2, 3, 4, 5];
    private static readonly int[] MinMaxSampleValues = [5, 3, 8, 1, 9];
    private static readonly int[] ThreeElementValues = [1, 2, 3];

    [Fact]
    public void Query_SumOperation_FullRange_ReturnsTotalSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(SumSampleValues);

        var actual = tree.Query(0, LastIndex);
        Assert.Equal(SumOfSample, actual);
    }

    [Fact]
    public void Query_SumOperation_PartialRange_ReturnsRangeSum()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(SumSampleValues);

        var actual = tree.Query(1, PartialRangeRightIndex);
        Assert.Equal(PartialRangeSum, actual);
    }

    [Fact]
    public void Query_SumOperation_SingleElementRange_ReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(SumSampleValues);

        var actual = tree.Query(MiddleIndex, MiddleIndex);
        Assert.Equal(ValueAtMiddleIndex, actual);
    }

    [Fact]
    public void Update_SumOperation_ChangesOnlyQueriesCoveringThatIndex()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(SumSampleValues);

        tree.Update(MiddleIndex, UpdatedValue);

        var updatedElementQuery = tree.Query(MiddleIndex, MiddleIndex);
        Assert.Equal(UpdatedValue, updatedElementQuery);
        var firstTwoElementsQuery = tree.Query(0, 1);
        Assert.Equal(SumOfFirstTwoElements, firstTwoElementsQuery);
        var fullRangeQuery = tree.Query(0, LastIndex);
        Assert.Equal(SumAfterUpdate, fullRangeQuery);
    }

    [Fact]
    public void Query_MinOperation_ReturnsSmallestInRange()
    {
        var tree = new SegmentTree<int, MinOperation<int>>(MinMaxSampleValues);

        var fullRangeMin = tree.Query(0, LastIndex);
        Assert.Equal(1, fullRangeMin);
        var firstTwoMin = tree.Query(0, 1);
        Assert.Equal(MinOfFirstTwoElements, firstTwoMin);
    }

    [Fact]
    public void Query_MaxOperation_ReturnsLargestInRange()
    {
        var tree = new SegmentTree<int, MaxOperation<int>>(MinMaxSampleValues);

        var fullRangeMax = tree.Query(0, LastIndex);
        Assert.Equal(MaxOfSample, fullRangeMax);
        var firstTwoMax = tree.Query(0, 1);
        Assert.Equal(MaxOfFirstTwoElements, firstTwoMax);
    }

    [Fact]
    public void Update_MinOperation_LoweringAValueUpdatesRangeMinimum()
    {
        var tree = new SegmentTree<int, MinOperation<int>>(MinMaxSampleValues);

        tree.Update(MiddleIndex, LoweredValue);

        var actual = tree.Query(0, LastIndex);
        Assert.Equal(LoweredValue, actual);
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(ThreeElementValues);

        Assert.Equal(ThreeElementValues.Length, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(ThreeElementValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, ThreeElementValues.Length));
    }

    [Fact]
    public void Query_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(ThreeElementValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(LastIndexOfThreeElementArray, 1));
    }

    [Fact]
    public void Update_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(ThreeElementValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Update(ThreeElementValues.Length, AnyUpdateValue));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new SegmentTree<int, SumOperation<int>>([SingleElementValue]);

        var actual = tree.Query(0, 0);
        Assert.Equal(SingleElementValue, actual);
    }
}
