using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class LazySegmentTreeTests
{
    [Fact]
    public void Query_RangeAddSum_FullRange_ReturnsTotalSumBeforeAnyUpdate()
    {
        var tree = RangeAddSumTree();

        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedTotalOfRangeAddSumInitialValues);
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OnlyAffectsQueriesOverlappingTheUpdatedRange()
    {
        var tree = RangeAddSumTreeAfterFirstUpdate();

        AssertRangeQuery(tree, 0, 0, 1);
        AssertRangeQuery(tree, 1, Fixtures.FirstUpdateRangeEnd, Fixtures.ExpectedSumAfterFirstUpdate);
        AssertRangeQuery(tree, Fixtures.SecondHalfStart, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedSumOutsideUpdatedRange);
        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedTotalAfterFirstUpdate);
    }

    [Fact]
    public void UpdateRange_RangeAddSum_OverlappingUpdates_ComposeAdditively()
    {
        var tree = RangeAddSumTreeAfterTwoOverlappingUpdates();

        AssertRangeQuery(tree, Fixtures.SecondUpdateRangeStart, Fixtures.FirstUpdateRangeEnd, Fixtures.ExpectedOverlapSum);
        AssertRangeQuery(tree, Fixtures.SecondHalfStart, Fixtures.SecondUpdateRangeEnd, Fixtures.ExpectedSecondOnlySum);
        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedTotalAfterOverlappingUpdates);
    }

    [Fact]
    public void Query_RangeAssignMax_ReturnsMaxInRangeBeforeAnyUpdate()
    {
        var tree = RangeAssignMaxTree();

        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedMaxOfWholeRange);
        AssertRangeQuery(tree, 0, Fixtures.FirstThreeElementsEnd, Fixtures.ExpectedMaxOfFirstThreeElements);
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_AssignsValueAcrossWholeRange()
    {
        var tree = RangeAssignMaxTreeAfterFirstAssignment();

        AssertRangeQuery(tree, 1, Fixtures.FirstUpdateRangeEnd, Fixtures.AssignedMaxValue);
        AssertRangeQuery(tree, Fixtures.SecondHalfStart, Fixtures.LastIndexOfEightElementTree, Fixtures.ExpectedMaxOutsideAssignedRange);
        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.AssignedMaxValue);
    }

    [Fact]
    public void UpdateRange_RangeAssignMax_OverlappingAssignments_NewerAssignmentWins()
    {
        var tree = RangeAssignMaxTreeAfterTwoOverlappingAssignments();

        AssertRangeQuery(tree, Fixtures.SecondUpdateRangeStart, Fixtures.SecondUpdateRangeEnd, Fixtures.SecondAssignedValue);
        AssertRangeQuery(tree, 1, Fixtures.IndexBeforeSecondUpdateRange, Fixtures.AssignedMaxValue);
        AssertRangeQuery(tree, 0, Fixtures.LastIndexOfEightElementTree, Fixtures.AssignedMaxValue);
    }

    [Fact]
    public void Count_ReflectsConstructorInputLength()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(Fixtures.SmallTreeValues);

        Assert.Equal(Fixtures.SmallTreeValues.Length, tree.Count);
    }

    [Fact]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(Fixtures.SmallTreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Query(0, Fixtures.SmallTreeValues.Length));
    }

    [Fact]
    public void UpdateRange_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(Fixtures.SmallTreeValues);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => tree.UpdateRange(Fixtures.InvalidRangeLeft, 1, Fixtures.ArbitraryUpdateValue));
    }

    [Fact]
    public void Constructor_SingleElement_QueryReturnsThatElement()
    {
        var tree = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(Fixtures.SingleElementTreeValues);

        var singleElementQuery = tree.Query(0, 0);

        Assert.Equal(Fixtures.SingleElementTreeValues[0], singleElementQuery);
    }

    // RangeAssignMaxOperation<int>'s TUpdate is int? with NoUpdate = null; passing that
    // sentinel straight through as an "update" would let ApplyToNode compose it away any
    // update already pending on a node before it's pushed to that node's children - guarded
    // at the public entry point rather than left as a silent-wrong-answer trap.
    [Fact]
    public void UpdateRange_RangeAssignMax_NoUpdateSentinel_ThrowsArgumentException()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(Fixtures.NoUpdateSentinelInitialValues);

        Assert.Throws<ArgumentException>(
            () => tree.UpdateRange(0, Fixtures.NoUpdateSentinelInitialValues.Length - 1, null));
    }

    // Every node above the leaves is TOperation.Combine of its two children, so a two-element
    // tree's root is exactly the operation's own Combine of the pair, and an eight-element
    // tree's root is that same combine folded over the whole input. Asserted against the
    // operation rather than against a hand-summed literal, which is the whole claim being made.
    [Fact]
    public void Combine_ChildrenOfEveryInternalNode_IsTheOperationsOwnCombine()
    {
        var pair = new LazySegmentTree<int, int, RangeAddSumOperation<int>>(Fixtures.CombinedPairValues);

        Assert.Equal(
            RangeAddSumOperation<int>.Combine(Fixtures.CombinedPairValues[0], Fixtures.CombinedPairValues[1]),
            pair.Query(0, Fixtures.CombinedPairValues.Length - 1));

        var tree = RangeAddSumTree();

        Assert.Equal(
            Fixtures.RangeAddSumInitialValues.Aggregate(
                (left, right) => RangeAddSumOperation<int>.Combine(left, right)),
            tree.Query(0, Fixtures.LastIndexOfEightElementTree));
    }

    // A query answers its disjoint branches with TOperation.Identity, so a max tree over values
    // that all sit below default still reports its true max: RangeAssignMaxOperation<int>'s
    // identity is int.MinValue, and a default(int) standing in for it would win the combine
    // instead of losing to every element.
    [Fact]
    public void Identity_DisjointQueryBranch_IsTheOperationsOwnIdentity()
    {
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(Fixtures.AllBelowDefaultValues);

        Assert.Equal(
            Fixtures.ExpectedMaxOfAllBelowDefaultValues,
            tree.Query(0, Fixtures.AllBelowDefaultValues.Length - 1));
        Assert.Equal(Fixtures.ExpectedMaxOfFirstTwoBelowDefaultValues, tree.Query(0, 1));
    }

    // The two configurations this file exercises, each built from the one before it: a test
    // asks for the state it queries by name instead of repeating the updates that reach it.
    private static LazySegmentTree<int, int, RangeAddSumOperation<int>> RangeAddSumTree() =>
        new(Fixtures.RangeAddSumInitialValues);

    private static LazySegmentTree<int, int, RangeAddSumOperation<int>> RangeAddSumTreeAfterFirstUpdate()
    {
        var tree = RangeAddSumTree();

        tree.UpdateRange(1, Fixtures.FirstUpdateRangeEnd, Fixtures.FirstUpdateDelta);

        return tree;
    }

    private static LazySegmentTree<int, int, RangeAddSumOperation<int>> RangeAddSumTreeAfterTwoOverlappingUpdates()
    {
        var tree = RangeAddSumTreeAfterFirstUpdate();

        tree.UpdateRange(Fixtures.SecondUpdateRangeStart, Fixtures.SecondUpdateRangeEnd, Fixtures.SecondUpdateDelta);

        return tree;
    }

    private static LazySegmentTree<int, int?, RangeAssignMaxOperation<int>> RangeAssignMaxTree() =>
        new(Fixtures.RangeAssignMaxInitialValues);

    private static LazySegmentTree<int, int?, RangeAssignMaxOperation<int>> RangeAssignMaxTreeAfterFirstAssignment()
    {
        var tree = RangeAssignMaxTree();

        tree.UpdateRange(1, Fixtures.FirstUpdateRangeEnd, Fixtures.AssignedMaxValue);

        return tree;
    }

    private static LazySegmentTree<int, int?, RangeAssignMaxOperation<int>> RangeAssignMaxTreeAfterTwoOverlappingAssignments()
    {
        var tree = RangeAssignMaxTreeAfterFirstAssignment();

        tree.UpdateRange(Fixtures.SecondUpdateRangeStart, Fixtures.SecondUpdateRangeEnd, Fixtures.SecondAssignedValue);

        return tree;
    }

    // Both configurations answer a range query the same way, so one assertion covers both:
    // `expected` is what the tree owes for the range, whatever operation it was built with.
    private static void AssertRangeQuery<Element, TUpdate, TOperation>(
        LazySegmentTree<Element, TUpdate, TOperation> tree, int low, int high, Element expected)
        where TOperation : struct, IRangeUpdateOperation<Element, TUpdate>
    {
        var actual = tree.Query(low, high);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// The values these tests run on and the results that follow from them: the two
    /// eight-element ranges, the small and single-element trees the bounds tests use,
    /// and the index bounds and expected values that belong to each. One holder rather
    /// than thirty-three members loose in the test class, so a reader follows the numbers
    /// in one place and the tests above read as the call sequences they are.
    /// </summary>
    private static class Fixtures
    {
        public static readonly int[] RangeAddSumInitialValues = [1, 2, 3, 4, 5, 6, 7, 8];
        public static readonly int[] RangeAssignMaxInitialValues = [5, 3, 8, 1, 9, 2, 7, 4];
        public static readonly int[] SmallTreeValues = [1, 2, 3];
        public static readonly int[] SingleElementTreeValues = [42];
        public static readonly int[] NoUpdateSentinelInitialValues = [5, 3, 8, 1];
        public static readonly int[] CombinedPairValues = [3, 4];

        // Every value below default, so a disjoint query branch answered with default(int)
        // rather than with the max operation's own int.MinValue identity would win the combine.
        public static readonly int[] AllBelowDefaultValues = [-5, -8, -3];

        // Index bounds shared by the tests above: every RangeAddSum/RangeAssignMax
        // test applies its first update over [1, FirstUpdateRangeEnd] and, where a
        // second update is layered on top, over [SecondUpdateRangeStart, SecondUpdateRangeEnd].
        public const int LastIndexOfEightElementTree = 7;
        public const int FirstUpdateRangeEnd = 4;
        public const int SecondUpdateRangeStart = 3;
        public const int SecondUpdateRangeEnd = 6;
        public const int SecondHalfStart = 5;
        public const int FirstThreeElementsEnd = 2;
        public const int IndexBeforeSecondUpdateRange = 2;
        public const int InvalidRangeLeft = 2;

        public const int FirstUpdateDelta = 10;
        public const int SecondUpdateDelta = 5;
        public const int AssignedMaxValue = 100;
        public const int SecondAssignedValue = 50;
        public const int ArbitraryUpdateValue = 5;

        public const int ExpectedTotalOfRangeAddSumInitialValues = 36;
        public const int ExpectedSumAfterFirstUpdate = 54;
        public const int ExpectedSumOutsideUpdatedRange = 21;
        public const int ExpectedTotalAfterFirstUpdate = 76;
        public const int ExpectedOverlapSum = 39;
        public const int ExpectedSecondOnlySum = 23;
        public const int ExpectedTotalAfterOverlappingUpdates = 96;
        public const int ExpectedMaxOfWholeRange = 9;
        public const int ExpectedMaxOfFirstThreeElements = 8;
        public const int ExpectedMaxOutsideAssignedRange = 7;
        public const int ExpectedMaxOfAllBelowDefaultValues = -3;
        public const int ExpectedMaxOfFirstTwoBelowDefaultValues = -5;
    }
}
