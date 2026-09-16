using DSAExperimentation.LeetCode.AlternatingGroupsIII;

namespace DSAExperimentation.LeetCode.Tests.AlternatingGroupsIII;

// The seam between AlternatingGroupsIIISolution's two query processors.
// NumberOfAlternatingGroupsByBruteForce walks every circular window of the circle
// directly; NumberOfAlternatingGroupsByRunLengthFenwick hands the same circle to
// AlternatingRunLedger, which composes a sorted DataStructures.DynamicArray of bad
// edges, BinarySearch.LowerBound for the insertion point, and two
// DataStructures.FenwickTree instances (runs-by-length and length-sum-by-length)
// that a size query reads with two prefix lookups each.
//
// The ledger is the only structure whose per-repaint state has to stay consistent
// across all four of those primitives at once: a repaint locates one bad edge with
// LowerBound, inserts or removes it from the DynamicArray, and applies three point
// updates spread over the Fenwick pair. The published-example suite pins each arm
// separately; agreement across a long repaint lifecycle is the only thing that
// checks the ledger's bookkeeping still describes the circle.
public sealed partial class AlternatingRunFenwickSeamTests
{
    // A fully alternating circle has no bad edge at all, the state AlternatingRunLedger
    // answers from an implicit run it never stores.
    [Fact]
    public void CountWindows_FullyAlternatingCircle_ReportsEveryStart()
    {
        int[][] queries = [CountWindow(2), CountWindow(3)];

        Assert.Equal(new[] { 4, 4 }, CountBothWays([0, 1, 0, 1], queries).Composed);
    }

    [Fact]
    public void CountWindows_UniformCircle_ReportsNoWindowPastLengthOne()
    {
        int[][] queries = [CountWindow(2), CountWindow(1), CountWindow(3)];

        var answers = CountBothWays([0, 0, 0, 0], queries);

        Assert.Equal(new[] { 0, 4, 0 }, answers.Reference);
        Assert.Equal(answers.Reference, answers.Composed);
    }

    [Fact]
    public void Repaint_SplitsTheOneImplicitRun_MatchesWindowScan()
    {
        int[][] queries =
        [
            Repaint(index: 2, color: 0),
            CountWindow(2),
            CountWindow(4),
            Repaint(index: 1, color: 1),
            CountWindow(2),
        ];

        AssertSameAnswers([0, 1, 1, 0], queries);
    }

    [Fact]
    public void Repaint_MergesTwoRunsBack_MatchesWindowScan()
    {
        int[][] queries =
        [
            Repaint(index: 1, color: 0),
            CountWindow(2),
            Repaint(index: 1, color: 1),
            CountWindow(3),
            Repaint(index: 3, color: 0),
            CountWindow(3),
        ];

        AssertSameAnswers([0, 1, 0, 0], queries);
    }

    [Fact]
    public void Repaint_ToTheColorAlreadyHeld_LeavesTheLedgerUnchanged()
    {
        int[][] queries =
        [
            CountWindow(2),
            Repaint(index: 0, color: 0),
            Repaint(index: 2, color: 1),
            CountWindow(2),
            CountWindow(4),
        ];

        AssertSameAnswers([0, 1, 0, 1], queries);
    }

    // Every window size the problem allows shares one ledger, so sizes far apart from
    // each other must read the same Fenwick pair without one query disturbing the next.
    [Fact]
    public void CountWindows_EveryAllowedSizeInOneLifecycle_MatchesWindowScan()
    {
        int[][] queries =
        [
            CountWindow(1),
            CountWindow(2),
            CountWindow(3),
            CountWindow(4),
            CountWindow(5),
            CountWindow(6),
            CountWindow(7),
        ];

        AssertSameAnswers([0, 1, 1, 0, 1, 0, 0, 1], queries);
    }

    private static void AssertSameAnswers(int[] colors, int[][] queries)
    {
        var answers = CountBothWays(colors, queries);

        Assert.Equal(answers.Reference, answers.Composed);
    }

    private static (IList<int> Reference, IList<int> Composed) CountBothWays(int[] colors, int[][] queries) => (
        AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByBruteForce(colors, queries),
        AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByRunLengthFenwick(colors, queries));

    private static int[] CountWindow(int size) => [1, size];

    private static int[] Repaint(int index, int color) => [2, index, color];
}
