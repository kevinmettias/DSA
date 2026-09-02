using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.Tests.DataStructures.RangeFenwickTree;

public sealed partial class RangeFenwickTreeTests
{
    private const int TreeSizeFive = 5;
    private const int TreeSizeSeven = 7;
    private const int TreeSizeThree = 3;
    private const int LastIndex = 4;
    private const int SumOfAllInitialValues = 15;

    private const int OverlapRangeLow = 1;
    private const int OverlapRangeHigh = 3;
    private const int OverlapRangeAddValue = 10;

    private static readonly int[] InitialValuesOneThroughFive = [1, 2, 3, 4, 5];

    private readonly record struct RangeAddQueryExpectations(int Before, int Within, int AtLastIndex, int Full);

    private static void AssertQuery(RangeFenwickTree<int, ScaledSumOperation<int>> tree, int low, int high, int expected)
    {
        var actual = tree.Query(low, high);
        Assert.Equal(expected, actual);
    }

    private static void AssertOverlapRangeAddQueries(
        RangeFenwickTree<int, ScaledSumOperation<int>> tree,
        RangeAddQueryExpectations expected)
    {
        tree.RangeAdd(OverlapRangeLow, OverlapRangeHigh, OverlapRangeAddValue);

        AssertQuery(tree, 0, 0, expected.Before);
        AssertQuery(tree, OverlapRangeLow, OverlapRangeHigh, expected.Within);
        AssertQuery(tree, LastIndex, LastIndex, expected.AtLastIndex);
        AssertQuery(tree, 0, LastIndex, expected.Full);
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtZeroEverywhere()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeFive);

        AssertQuery(tree, 0, LastIndex, 0);
    }

    [Fact]
    public void RangeAdd_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeFive);

        const int SumWithinRange = 30;
        const int SumOverWholeTree = 30;
        AssertOverlapRangeAddQueries(
            tree,
            new RangeAddQueryExpectations(Before: 0, Within: SumWithinRange, AtLastIndex: 0, Full: SumOverWholeTree));
    }

    [Fact]
    public void RangeAdd_OverlappingUpdates_ComposeAdditively()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeFive);

        const int SecondRangeLow = 2;
        const int SecondRangeAddValue = 5;
        tree.RangeAdd(OverlapRangeLow, OverlapRangeHigh, OverlapRangeAddValue);
        tree.RangeAdd(SecondRangeLow, LastIndex, SecondRangeAddValue);

        const int SumOverOverlapOfBothRanges = 30;
        const int SumAtLastIndexAfterBothRanges = 5;
        const int SumOverWholeTreeAfterBothRanges = 45;
        AssertQuery(tree, SecondRangeLow, OverlapRangeHigh, SumOverOverlapOfBothRanges);
        AssertQuery(tree, LastIndex, LastIndex, SumAtLastIndexAfterBothRanges);
        AssertQuery(tree, 0, LastIndex, SumOverWholeTreeAfterBothRanges);
    }

    [Fact]
    public void RangeAdd_ExtendsToTheLastIndex_DoesNotThrowOnBoundaryCancellation()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeFive);

        const int FullRangeAddValue = 7;
        tree.RangeAdd(0, LastIndex, FullRangeAddValue);

        const int SumOverWholeTreeAfterFullRangeAdd = 35;
        AssertQuery(tree, 0, LastIndex, SumOverWholeTreeAfterFullRangeAdd);
    }

    [Fact]
    public void Constructor_FromInitialValues_QueryMatchesRangeSum()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        const int SumOfMiddleThreeInitialValues = 9;
        AssertQuery(tree, 0, LastIndex, SumOfAllInitialValues);
        AssertQuery(tree, OverlapRangeLow, OverlapRangeHigh, SumOfMiddleThreeInitialValues);
    }

    [Fact]
    public void RangeAdd_AfterConstructorFromInitialValues_ComposesWithInitialValues()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        const int SumWithinRangeAfterInitialValues = 39;
        const int SumAtLastIndexAfterInitialValues = 5;
        const int SumOverWholeTreeAfterInitialValues = 45;
        AssertOverlapRangeAddQueries(
            tree,
            new RangeAddQueryExpectations(
                Before: 1,
                Within: SumWithinRangeAfterInitialValues,
                AtLastIndex: SumAtLastIndexAfterInitialValues,
                Full: SumOverWholeTreeAfterInitialValues));
    }

    [Fact]
    public void PrefixQuery_ReturnsRunningSumUpToIndex()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        const int PrefixQueryIndex = 2;
        const int RunningSumThroughIndexTwo = 6;
        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(RunningSumThroughIndexTwo, tree.PrefixQuery(PrefixQueryIndex));
        Assert.Equal(SumOfAllInitialValues, tree.PrefixQuery(LastIndex));
    }

    [Fact]
    public void Query_SingleIndex_ActsAsAPointQuery()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        tree.RangeAdd(OverlapRangeLow, OverlapRangeHigh, OverlapRangeAddValue);

        const int PointValueAtIndexOne = 12;
        const int PointValueAtIndexThree = 14;
        AssertQuery(tree, OverlapRangeLow, OverlapRangeLow, PointValueAtIndexOne);
        AssertQuery(tree, OverlapRangeHigh, OverlapRangeHigh, PointValueAtIndexThree);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeSeven);

        Assert.Equal(TreeSizeSeven, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeThree);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, TreeSizeThree));
    }

    [Fact]
    public void RangeAdd_RightOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(TreeSizeThree);

        const int OutOfRangeAddValue = 5;
        Assert.Throws<ArgumentOutOfRangeException>(() => tree.RangeAdd(0, TreeSizeThree, OutOfRangeAddValue));
    }
}
