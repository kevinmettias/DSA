using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

// The one segment tree's contract, run over its configurations: the trivial action under three
// monoids (sum, minimum, maximum) and the two range actions this folder ships (add-then-sum and
// assign-then-max). A seeded script of updates must leave every range query equal to the monoid's
// fold of a plain array that took the same updates, where a range update reaches each of its
// elements as the action's own ApplyUpdate over a range of one - the law ApplyUpdate states. What
// only a configuration with an action has, UpdateRange, runs over the two that have one, and that
// the trivial action keeps no pending tags is read off what its constructor allocates.
public sealed partial class SegmentTreeTests
{
    private const string Sum = nameof(Addition<int>);
    private const string Min = nameof(Minimum<int>);
    private const string Max = nameof(Maximum<int>);
    private const string AddSum = nameof(RangeAddSumOperation<int>);
    private const string AssignMax = nameof(RangeAssignMaxOperation<int>);

    private const int ScriptLength = 600;
    private const int ScriptSeed = 1_313;
    private const int ValueBound = 50;

    // Not a power of two, so the halves the tree splits a range into are uneven somewhere.
    private const int ScriptElementCount = 37;

    // One update in this many is a point update in the mixed script, the rest range updates.
    private const int PointUpdateOdds = 3;

    public static TheoryData<string> Configurations => [Sum, Min, Max, AddSum, AssignMax];

    public static TheoryData<string> ActionConfigurations => [AddSum, AssignMax];

    public static TheoryData<string> MaximumConfigurations => [Max, AssignMax];

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Count_IsTheInitialLength(string configuration) =>
        Assert.Equal(Samples.Values.Length, TreeOf(configuration, Samples.Values).Count);

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Query_EveryRangeOfAFreshTree_IsTheFoldOfItsElements(string configuration)
    {
        var (expected, actual) = EveryRange(TreeOf(configuration, Samples.Values), Samples.Values);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Update_SeededScript_EveryQueryAgreesWithTheElements(string configuration)
    {
        var elements = ScriptElements();
        var (expected, actual) = PointScript(TreeOf(configuration, elements), elements);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ActionConfigurations))]
    public void UpdateRange_SeededScriptMixedWithPointUpdates_EveryQueryAgreesWithTheElements(
        string configuration)
    {
        var elements = ScriptElements();
        var (expected, actual) = MixedScript(RangeTreeOf(configuration, elements), elements);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Constructor_SingleElement_QueryReturnsThatElement(string configuration) =>
        Assert.Equal(Samples.SingleValue, TreeOf(configuration, [Samples.SingleValue]).Query(0, 0));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Query_OutOfRangeRight_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, Samples.Values).Query(0, Samples.Values.Length));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Query_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, Samples.Values)
                .Query(Samples.ReversedLeft, Samples.ReversedRight));

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Update_IndexOutOfRange_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TreeOf(configuration, Samples.Values)
                .Update(Samples.Values.Length, Samples.SingleValue));

    [Theory]
    [MemberData(nameof(ActionConfigurations))]
    public void UpdateRange_LeftGreaterThanRight_ThrowsArgumentOutOfRangeException(string configuration) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RangeTreeOf(configuration, Samples.Values).UpdateRange(
                Samples.ReversedLeft, Samples.ReversedRight, Samples.SingleValue, [.. Samples.Values]));

    // A query answers its disjoint branches with the monoid's identity, so a maximum over values
    // that all sit below default still reports its true maximum: the identity is int.MinValue, and
    // a default(int) standing in for it would win the combine instead of losing to every element.
    [Theory]
    [MemberData(nameof(MaximumConfigurations))]
    public void Query_ValuesBelowDefault_DisjointBranchesAnswerTheMonoidsIdentity(
        string configuration)
    {
        var tree = TreeOf(configuration, Samples.BelowDefaultValues);

        Assert.Equal(
            (Samples.MaximumOfBelowDefaultValues, Samples.MaximumOfFirstTwoBelowDefaultValues),
            (tree.Query(0, Samples.BelowDefaultValues.Length - 1), tree.Query(0, 1)));
    }

    // RangeAssignMaxOperation<int>'s update type is int? with NoUpdate = null; passing that
    // sentinel as an update would let a node compose away an update already pending on it before it
    // reaches the node's children - guarded at the public entry point rather than left as a silent
    // trap.
    [Fact]
    public void UpdateRange_NoUpdateSentinel_ThrowsArgumentException()
    {
        var tree = new SegmentTree<int, int?, RangeAssignMaxOperation<int>>(Samples.Values);

        Assert.Throws<ArgumentException>(() => tree.UpdateRange(0, Samples.Values.Length - 1, null));
    }

    [Fact]
    public void UpdateRange_OverlappingAdds_ComposeAdditively()
    {
        var tree = new SegmentTree<int, int, RangeAddSumOperation<int>>(Samples.Ascending);
        tree.UpdateRange(1, Samples.FirstUpdateEnd, Samples.FirstDelta);
        tree.UpdateRange(Samples.SecondUpdateStart, Samples.SecondUpdateEnd, Samples.SecondDelta);

        Assert.Equal(
            (Samples.OverlapSum, Samples.SecondOnlySum, Samples.TotalAfterBothAdds),
            (tree.Query(Samples.SecondUpdateStart, Samples.FirstUpdateEnd),
                tree.Query(Samples.FirstUpdateEnd + 1, Samples.SecondUpdateEnd),
                tree.Query(0, Samples.Ascending.Length - 1)));
    }

    [Fact]
    public void UpdateRange_OverlappingAssignments_NewerAssignmentWins()
    {
        var tree = new SegmentTree<int, int?, RangeAssignMaxOperation<int>>(Samples.Values);
        tree.UpdateRange(1, Samples.FirstUpdateEnd, Samples.FirstAssignment);
        tree.UpdateRange(
            Samples.SecondUpdateStart, Samples.SecondUpdateEnd, Samples.SecondAssignment);

        Assert.Equal(
            (Samples.SecondAssignment, Samples.FirstAssignment),
            (tree.Query(Samples.SecondUpdateStart, Samples.SecondUpdateEnd),
                tree.Query(1, Samples.SecondUpdateStart - 1)));
    }

    // An add over the whole range leaves its tag pending at the root, so the point update below it
    // must push that tag down first: otherwise the root recombines from children that never saw the
    // add, and the tag, still pending, later lands on the value the point update just wrote.
    [Fact]
    public void Update_UnderAPendingRangeUpdate_KeepsTheOtherElementsUpdated()
    {
        var tree = new SegmentTree<int, int, RangeAddSumOperation<int>>(Samples.Ascending);
        tree.UpdateRange(0, Samples.Ascending.Length - 1, Samples.FirstDelta);
        tree.Update(Samples.RewrittenIndex, 0);

        Assert.Equal(
            (Samples.TotalAfterAddThenRewrite, 0, Samples.NeighbourAfterAdd),
            (tree.Query(0, Samples.Ascending.Length - 1),
                tree.Query(Samples.RewrittenIndex, Samples.RewrittenIndex),
                tree.Query(Samples.RewrittenIndex + 1, Samples.RewrittenIndex + 1)));
    }

    // The trivial action has nothing to defer, so its tree allocates only its values: a tree with
    // an action allocates one more arena of the same length, its tags, and what the trivial tree
    // allocates must come to less than that values arena plus the two small objects around it.
    [Fact]
    public void Constructor_TrivialAction_AllocatesNoPendingTags()
    {
        var initial = new int[Allocation.ElementCount];
        var pointBytes = AllocatedBy(() => new SegmentTree<int, Addition<int>>(initial));
        var rangeBytes = AllocatedBy(
            () => new SegmentTree<int, int, RangeAddSumOperation<int>>(initial));

        Assert.InRange(pointBytes, 0, rangeBytes - pointBytes + Allocation.NonArenaAllowance);
    }

    // Every range [left, right] of the elements, with the tree's answer beside the fold's.
    private static (List<int> Expected, List<int> Actual) EveryRange(ITreeView tree, int[] elements)
    {
        var expected = new List<int>();
        var actual = new List<int>();

        for (var left = 0; left < elements.Length; left++)
        {
            for (var right = left; right < elements.Length; right++)
            {
                expected.Add(tree.Fold(elements, left, right));
                actual.Add(tree.Query(left, right));
            }
        }

        return (expected, actual);
    }

    // Point updates at seeded positions, each followed by a query over a seeded range.
    private static (List<int> Expected, List<int> Actual) PointScript(ITreeView tree, int[] elements)
    {
        var random = new Random(ScriptSeed);
        var expected = new List<int>();
        var actual = new List<int>();

        for (var step = 0; step < ScriptLength; step++)
        {
            var index = random.Next(elements.Length);
            var value = random.Next(-ValueBound, ValueBound);
            tree.Update(index, value);
            elements[index] = value;

            var (left, right) = SeededRange(random, elements.Length);
            expected.Add(tree.Fold(elements, left, right));
            actual.Add(tree.Query(left, right));
        }

        return (expected, actual);
    }

    // Range updates mixed with point updates, two to one, each followed by a query over a seeded
    // range.
    private static (List<int> Expected, List<int> Actual) MixedScript(IRangeTreeView tree, int[] elements)
    {
        var random = new Random(ScriptSeed);
        var expected = new List<int>();
        var actual = new List<int>();

        for (var step = 0; step < ScriptLength; step++)
        {
            ApplySeededUpdate(tree, elements, random);

            var (left, right) = SeededRange(random, elements.Length);
            expected.Add(tree.Fold(elements, left, right));
            actual.Add(tree.Query(left, right));
        }

        return (expected, actual);
    }

    private static void ApplySeededUpdate(IRangeTreeView tree, int[] elements, Random random)
    {
        var (left, right) = SeededRange(random, elements.Length);
        var isPointUpdate = random.Next(PointUpdateOdds) == 0;

        if (isPointUpdate)
        {
            var value = random.Next(-ValueBound, ValueBound);
            tree.Update(left, value);
            elements[left] = value;
            return;
        }

        tree.UpdateRange(left, right, SeededRangeUpdate(random), elements);
    }

    // A seeded update other than 0, which the add-then-sum action reserves as its NoUpdate.
    private static int SeededRangeUpdate(Random random)
    {
        var value = random.Next(-ValueBound, ValueBound - 1);

        if (value >= 0)
        {
            value++;
        }

        return value;
    }

    private static (int Left, int Right) SeededRange(Random random, int length)
    {
        var left = random.Next(length);

        return (left, random.Next(left, length));
    }

    private static int[] ScriptElements()
    {
        var random = new Random(ScriptSeed);

        return [.. Enumerable.Range(0, ScriptElementCount).Select(_ => random.Next(-ValueBound, ValueBound))];
    }

    // Bytes the calling thread allocates while making one object, kept alive until it is counted.
    // The first pass loads and compiles what make() runs; the last pass is the one returned.
    private static long AllocatedBy(Func<object> make)
    {
        var allocated = 0L;

        for (var pass = 0; pass < Allocation.Passes; pass++)
        {
            var before = GC.GetAllocatedBytesForCurrentThread();
            var made = make();
            allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            GC.KeepAlive(made);
        }

        return allocated;
    }

    private static ITreeView TreeOf(string configuration, int[] initial) => configuration switch
    {
        Sum => View(new SegmentTree<int, Addition<int>>(initial)),
        Min => View(new SegmentTree<int, Minimum<int>>(initial)),
        Max => View(new SegmentTree<int, Maximum<int>>(initial)),
        _ => RangeTreeOf(configuration, initial),
    };

    private static IRangeTreeView RangeTreeOf(string configuration, int[] initial) => configuration switch
    {
        AddSum => RangeView(new SegmentTree<int, int, RangeAddSumOperation<int>>(initial), value => value),
        _ => RangeView<int?, RangeAssignMaxOperation<int>>(
            new SegmentTree<int, int?, RangeAssignMaxOperation<int>>(initial), value => value),
    };

    private static TreeView<TUpdate, TOperation, TAction> View<TUpdate, TOperation, TAction>(
        SegmentTree<int, TUpdate, TOperation, TAction> tree)
        where TOperation : struct, IMonoid<int>
        where TAction : struct, IRangeAction<int, TUpdate> => new(tree);

    private static RangeTreeView<TUpdate, TOperation> RangeView<TUpdate, TOperation>(
        SegmentTree<int, TUpdate, TOperation> tree, Func<int, TUpdate> updateOf)
        where TOperation : struct, IRangeUpdateOperation<int, TUpdate> => new(tree, updateOf);

    // The monoid's fold of elements[left..right], in index order: what a query must answer.
    private static int FoldOf<TOperation>(int[] elements, int left, int right)
        where TOperation : struct, IMonoid<int>
    {
        var combined = TOperation.Identity;

        for (var index = left; index <= right; index++)
        {
            combined = TOperation.Combine(combined, elements[index]);
        }

        return combined;
    }

    // What the contract reads of any configuration: the operations every one has, and the fold a
    // query must agree with.
    private interface ITreeView
    {
        int Count { get; }

        int Query(int left, int right);

        void Update(int index, int value);

        int Fold(int[] elements, int left, int right);
    }

    // A configuration with an action: a range update applied to the tree and, element by element,
    // to the plain array the tree must agree with.
    private interface IRangeTreeView : ITreeView
    {
        void UpdateRange(int left, int right, int value, int[] elements);
    }

    private sealed class TreeView<TUpdate, TOperation, TAction>(SegmentTree<int, TUpdate, TOperation, TAction> tree)
        : ITreeView
        where TOperation : struct, IMonoid<int>
        where TAction : struct, IRangeAction<int, TUpdate>
    {
        public int Count => tree.Count;

        public int Query(int left, int right) => tree.Query(left, right);

        public void Update(int index, int value) => tree.Update(index, value);

        public int Fold(int[] elements, int left, int right) => FoldOf<TOperation>(elements, left, right);
    }

    private sealed class RangeTreeView<TUpdate, TOperation>(
        SegmentTree<int, TUpdate, TOperation> tree, Func<int, TUpdate> updateOf) : IRangeTreeView
        where TOperation : struct, IRangeUpdateOperation<int, TUpdate>
    {
        public int Count => tree.Count;

        public int Query(int left, int right) => tree.Query(left, right);

        public void Update(int index, int value) => tree.Update(index, value);

        public int Fold(int[] elements, int left, int right) => FoldOf<TOperation>(elements, left, right);

        public void UpdateRange(int left, int right, int value, int[] elements)
        {
            var update = updateOf(value);
            tree.UpdateRange(left, right, update);

            for (var index = left; index <= right; index++)
            {
                elements[index] = TOperation.ApplyUpdate(elements[index], update, 1);
            }
        }
    }

    // The sample inputs the named examples build their trees from, and the values that follow from
    // each: one holder, so the numbers a reader has to follow sit in one place.
    private static class Samples
    {
        public static readonly int[] Values = [5, 3, 8, 1, 9, 2, 7, 4];
        public static readonly int[] Ascending = [1, 2, 3, 4, 5, 6, 7, 8];

        // Every value below default, so a disjoint branch answered with default(int) rather than
        // with the maximum's own int.MinValue identity would win the combine.
        public static readonly int[] BelowDefaultValues = [-5, -8, -3];

        public const int SingleValue = 42;
        public const int ReversedLeft = 2;
        public const int ReversedRight = 1;

        public const int FirstUpdateEnd = 4;
        public const int SecondUpdateStart = 3;
        public const int SecondUpdateEnd = 6;
        public const int FirstDelta = 10;
        public const int SecondDelta = 5;
        public const int FirstAssignment = 100;
        public const int SecondAssignment = 50;
        public const int RewrittenIndex = 2;

        public const int OverlapSum = 39;
        public const int SecondOnlySum = 23;
        public const int TotalAfterBothAdds = 96;
        public const int TotalAfterAddThenRewrite = 103;
        public const int NeighbourAfterAdd = 14;
        public const int MaximumOfBelowDefaultValues = -3;
        public const int MaximumOfFirstTwoBelowDefaultValues = -5;
    }

    // The allocation fact's sizes.
    private static class Allocation
    {
        // Large enough that an arena of tags, even of one-byte NoUpdate values, outweighs the
        // allowance below several times over.
        public const int ElementCount = 1_024;

        // Room for the tree object and its arena's wrapper, far below any arena of ElementCount
        // slots.
        public const int NonArenaAllowance = 512;

        // A warm-up pass, then the pass that is counted.
        public const int Passes = 2;
    }
}
