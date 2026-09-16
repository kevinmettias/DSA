using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.Tests.DataStructures.RangeFenwickTree;

public sealed partial class RangeFenwickTreeTests
{
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
        tree.RangeAdd(Fixtures.OverlapRangeLow, Fixtures.OverlapRangeHigh, Fixtures.OverlapRangeAddValue);

        AssertQuery(tree, 0, 0, expected.Before);
        AssertQuery(tree, Fixtures.OverlapRangeLow, Fixtures.OverlapRangeHigh, expected.Within);
        AssertQuery(tree, Fixtures.LastIndex, Fixtures.LastIndex, expected.AtLastIndex);
        AssertQuery(tree, 0, Fixtures.LastIndex, expected.Full);
    }

    [Fact]
    public void Constructor_SizeOnly_StartsAtZeroEverywhere()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeFive);

        AssertQuery(tree, 0, Fixtures.LastIndex, 0);
    }

    [Fact]
    public void RangeAdd_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeFive);

        AssertOverlapRangeAddQueries(
            tree,
            new RangeAddQueryExpectations(
                Before: 0,
                Within: Fixtures.SumWithinRange,
                AtLastIndex: 0,
                Full: Fixtures.SumOverWholeTree));
    }

    [Fact]
    public void RangeAdd_OverlappingUpdates_ComposeAdditively()
    {
        var tree = TreeWithTwoOverlappingRangeAdds();

        AssertQuery(tree, Fixtures.SecondRangeLow, Fixtures.OverlapRangeHigh, Fixtures.SumOverOverlapOfBothRanges);
        AssertQuery(tree, Fixtures.LastIndex, Fixtures.LastIndex, Fixtures.SumAtLastIndexAfterBothRanges);
        AssertQuery(tree, 0, Fixtures.LastIndex, Fixtures.SumOverWholeTreeAfterBothRanges);
    }

    private static RangeFenwickTree<int, ScaledSumOperation<int>> TreeWithTwoOverlappingRangeAdds()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeFive);

        tree.RangeAdd(Fixtures.OverlapRangeLow, Fixtures.OverlapRangeHigh, Fixtures.OverlapRangeAddValue);
        tree.RangeAdd(Fixtures.SecondRangeLow, Fixtures.LastIndex, Fixtures.SecondRangeAddValue);

        return tree;
    }

    [Fact]
    public void RangeAdd_ExtendsToTheLastIndex_DoesNotThrowOnBoundaryCancellation()
    {
        var tree = TreeAfterFullRangeAdd();

        AssertQuery(tree, 0, Fixtures.LastIndex, Fixtures.SumOverWholeTreeAfterFullRangeAdd);
    }

    private static RangeFenwickTree<int, ScaledSumOperation<int>> TreeAfterFullRangeAdd()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeFive);

        tree.RangeAdd(0, Fixtures.LastIndex, Fixtures.FullRangeAddValue);

        return tree;
    }

    [Fact]
    public void Constructor_FromInitialValues_QueryMatchesRangeSum()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        AssertQuery(tree, 0, Fixtures.LastIndex, Fixtures.SumOfAllInitialValues);
        AssertQuery(tree, Fixtures.OverlapRangeLow, Fixtures.OverlapRangeHigh, Fixtures.SumOfMiddleThreeInitialValues);
    }

    [Fact]
    public void RangeAdd_AfterConstructorFromInitialValues_ComposesWithInitialValues()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        AssertOverlapRangeAddQueries(
            tree,
            new RangeAddQueryExpectations(
                Before: 1,
                Within: Fixtures.SumWithinRangeAfterInitialValues,
                AtLastIndex: Fixtures.SumAtLastIndexAfterInitialValues,
                Full: Fixtures.SumOverWholeTreeAfterInitialValues));
    }

    [Fact]
    public void PrefixQuery_ReturnsRunningSumUpToIndex()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        Assert.Equal(1, tree.PrefixQuery(0));
        Assert.Equal(Fixtures.RunningSumThroughIndexTwo, tree.PrefixQuery(Fixtures.PrefixQueryIndex));
        Assert.Equal(Fixtures.SumOfAllInitialValues, tree.PrefixQuery(Fixtures.LastIndex));
    }

    [Fact]
    public void Query_SingleIndex_ActsAsAPointQuery()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(InitialValuesOneThroughFive);

        tree.RangeAdd(Fixtures.OverlapRangeLow, Fixtures.OverlapRangeHigh, Fixtures.OverlapRangeAddValue);

        AssertQuery(tree, Fixtures.OverlapRangeLow, Fixtures.OverlapRangeLow, Fixtures.PointValueAtIndexOne);
        AssertQuery(tree, Fixtures.OverlapRangeHigh, Fixtures.OverlapRangeHigh, Fixtures.PointValueAtIndexThree);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeSeven);

        Assert.Equal(Fixtures.TreeSizeSeven, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeThree);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, Fixtures.TreeSizeThree));
    }

    [Fact]
    public void RangeAdd_RightOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var tree = new RangeFenwickTree<int, ScaledSumOperation<int>>(Fixtures.TreeSizeThree);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.RangeAdd(0, Fixtures.TreeSizeThree, Fixtures.OutOfRangeAddValue));
    }

    /// <summary>
    /// The sizes, ranges and expected sums these tests probe, named once so a second
    /// test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const int TreeSizeFive = 5;
        public const int TreeSizeSeven = 7;
        public const int TreeSizeThree = 3;
        public const int LastIndex = 4;
        public const int SumOfAllInitialValues = 15;
        public const int OverlapRangeLow = 1;
        public const int OverlapRangeHigh = 3;
        public const int OverlapRangeAddValue = 10;
        public const int SumWithinRange = 30;
        public const int SumOverWholeTree = 30;
        public const int SecondRangeLow = 2;
        public const int SecondRangeAddValue = 5;
        public const int SumOverOverlapOfBothRanges = 30;
        public const int SumAtLastIndexAfterBothRanges = 5;
        public const int SumOverWholeTreeAfterBothRanges = 45;
        public const int FullRangeAddValue = 7;
        public const int SumOverWholeTreeAfterFullRangeAdd = 35;
        public const int SumOfMiddleThreeInitialValues = 9;
        public const int SumWithinRangeAfterInitialValues = 39;
        public const int SumAtLastIndexAfterInitialValues = 5;
        public const int SumOverWholeTreeAfterInitialValues = 45;
        public const int PrefixQueryIndex = 2;
        public const int RunningSumThroughIndexTwo = 6;
        public const int PointValueAtIndexOne = 12;
        public const int PointValueAtIndexThree = 14;
        public const int OutOfRangeAddValue = 5;
    }
}
