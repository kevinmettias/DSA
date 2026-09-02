using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class LazySegmentTreeTests
{
    private static readonly int[] RangeAddSumInitialValues = [1, 2, 3, 4, 5, 6, 7, 8];
    private static readonly int[] RangeAssignMaxInitialValues = [5, 3, 8, 1, 9, 2, 7, 4];
    private static readonly int[] SmallTreeValues = [1, 2, 3];
    private static readonly int[] SingleElementTreeValues = [42];
    private static readonly int[] NoUpdateSentinelInitialValues = [5, 3, 8, 1];

    // Index bounds shared by the tests above: every RangeAddSum/RangeAssignMax
    // test applies its first update over [1, FirstUpdateRangeEnd] and, where a
    // second update is layered on top, over [SecondUpdateRangeStart, SecondUpdateRangeEnd].
    private const int LastIndexOfEightElementTree = 7;
    private const int FirstUpdateRangeEnd = 4;
    private const int SecondUpdateRangeStart = 3;
    private const int SecondUpdateRangeEnd = 6;
    private const int SecondHalfStart = 5;
    private const int FirstThreeElementsEnd = 2;
    private const int IndexBeforeSecondUpdateRange = 2;
    private const int InvalidRangeLeft = 2;

    private const int FirstUpdateDelta = 10;
    private const int SecondUpdateDelta = 5;
    private const int AssignedMaxValue = 100;
    private const int SecondAssignedValue = 50;
    private const int ArbitraryUpdateValue = 5;

    private const int ExpectedTotalOfRangeAddSumInitialValues = 36;
    private const int ExpectedSumAfterFirstUpdate = 54;
    private const int ExpectedSumOutsideUpdatedRange = 21;
    private const int ExpectedTotalAfterFirstUpdate = 76;
    private const int ExpectedOverlapSum = 39;
    private const int ExpectedSecondOnlySum = 23;
    private const int ExpectedTotalAfterOverlappingUpdates = 96;
    private const int ExpectedMaxOfWholeRange = 9;
    private const int ExpectedMaxOfFirstThreeElements = 8;
    private const int ExpectedMaxOutsideAssignedRange = 7;

    [Fact]
    public void Query_RangeAddSum_FullRange_ReturnsTotalSumBeforeAnyUpdate()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(RangeAddSumInitialValues);

        var total = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedTotalOfRangeAddSumInitialValues, total);
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(RangeAddSumInitialValues);

        tree.UpdateRange(1, FirstUpdateRangeEnd, FirstUpdateDelta);

        var beforeUpdatedRange = tree.Query(0, 0);
        Assert.Equal(1, beforeUpdatedRange);

        var withinUpdatedRange = tree.Query(1, FirstUpdateRangeEnd);
        Assert.Equal(ExpectedSumAfterFirstUpdate, withinUpdatedRange);

        var outsideUpdatedRange = tree.Query(SecondHalfStart, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedSumOutsideUpdatedRange, outsideUpdatedRange);

        var fullRangeAfterUpdate = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedTotalAfterFirstUpdate, fullRangeAfterUpdate);
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OverlappingUpdates_ComposeAdditively()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(RangeAddSumInitialValues);

        tree.UpdateRange(1, FirstUpdateRangeEnd, FirstUpdateDelta);
        tree.UpdateRange(SecondUpdateRangeStart, SecondUpdateRangeEnd, SecondUpdateDelta);

        var overlapSum = tree.Query(SecondUpdateRangeStart, FirstUpdateRangeEnd);
        Assert.Equal(ExpectedOverlapSum, overlapSum);

        var secondOnlySum = tree.Query(SecondHalfStart, SecondUpdateRangeEnd);
        Assert.Equal(ExpectedSecondOnlySum, secondOnlySum);

        var totalAfterOverlap = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedTotalAfterOverlappingUpdates, totalAfterOverlap);
    }

    [Fact]
    public void Query_RangeAssignMax_ReturnsMaxInRangeBeforeAnyUpdate()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(RangeAssignMaxInitialValues);

        var maxOfWholeRange = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedMaxOfWholeRange, maxOfWholeRange);

        var maxOfFirstThreeElements = tree.Query(0, FirstThreeElementsEnd);
        Assert.Equal(ExpectedMaxOfFirstThreeElements, maxOfFirstThreeElements);
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_AssignsValueAcrossWholeRange()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(RangeAssignMaxInitialValues);

        tree.UpdateRange(1, FirstUpdateRangeEnd, AssignedMaxValue);

        var withinAssignedRange = tree.Query(1, FirstUpdateRangeEnd);
        Assert.Equal(AssignedMaxValue, withinAssignedRange);

        var outsideAssignedRange = tree.Query(SecondHalfStart, LastIndexOfEightElementTree);
        Assert.Equal(ExpectedMaxOutsideAssignedRange, outsideAssignedRange);

        var fullRangeAfterAssign = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(AssignedMaxValue, fullRangeAfterAssign);
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_OverlappingAssignments_NewerAssignmentWins()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(RangeAssignMaxInitialValues);

        tree.UpdateRange(1, FirstUpdateRangeEnd, AssignedMaxValue);
        tree.UpdateRange(SecondUpdateRangeStart, SecondUpdateRangeEnd, SecondAssignedValue);

        var overlapAfterNewerAssignment = tree.Query(SecondUpdateRangeStart, SecondUpdateRangeEnd);
        Assert.Equal(SecondAssignedValue, overlapAfterNewerAssignment);

        var untouchedByNewerAssignment = tree.Query(1, IndexBeforeSecondUpdateRange);
        Assert.Equal(AssignedMaxValue, untouchedByNewerAssignment);

        var fullRangeAfterOverlap = tree.Query(0, LastIndexOfEightElementTree);
        Assert.Equal(AssignedMaxValue, fullRangeAfterOverlap);
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(SmallTreeValues);

        Assert.Equal(SmallTreeValues.Length, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(SmallTreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, SmallTreeValues.Length));
    }

    [Fact]
    public void UpdateRange_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(SmallTreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.UpdateRange(InvalidRangeLeft, 1, ArbitraryUpdateValue));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(SingleElementTreeValues);

        var singleElementQuery = tree.Query(0, 0);
        Assert.Equal(SingleElementTreeValues[0], singleElementQuery);
    }

    // RangeAssignMaxOperation<int>'s TUpdate is int? with NoUpdate = null; passing that
    // sentinel straight through as an "update" would let ApplyToNode compose it away any
    // update already pending on a node before it's pushed to that node's children - guarded
    // at the public entry point rather than left as a silent-wrong-answer trap.
    [Fact]
    public void UpdateRange_RangeAssignMax_NoUpdateSentinel_ThrowsArgumentException()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(NoUpdateSentinelInitialValues);

        Assert.Throws<ArgumentException>(() => tree.UpdateRange(0, NoUpdateSentinelInitialValues.Length - 1, null));
    }
}
