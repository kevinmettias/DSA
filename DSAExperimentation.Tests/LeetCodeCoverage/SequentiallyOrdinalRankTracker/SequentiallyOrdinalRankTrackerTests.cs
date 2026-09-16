using DSAExperimentation.LeetCode.SequentiallyOrdinalRankTracker;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SequentiallyOrdinalRankTracker;

// Harness only. Both strategies are SequentiallyOrdinalRankTrackerSolution's -
// this file replays call scripts against each IRankTracker instance, so a failure
// still names the strategy that broke even though the "input" here is a sequence
// of add/get calls rather than a single argument tuple, the same shape
// AllOneDataStructureTests and LRUCacheTests already use for their own
// instance-API problems. The pre-migration test only proved the two-heap tracker;
// the re-sort baseline (previously untested scaffolding inlined in the benchmark)
// gets that same coverage here for the first time. RankTrackerOp.Apply is pure
// dispatch, no ranking logic of its own.
public sealed partial class SequentiallyOrdinalRankTrackerTests
{
    public static TheoryData<RankTrackerOp[], string?[]> Examples =>
        new()
        {
            {
                // LeetCode's published call script, whose last two calls are two
                // consecutive gets with no add between them.
                [
                    RankTrackerOp.Add("bradford", 2),
                    RankTrackerOp.Add("branford", 3),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("alps", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("orl", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("orlando", 3),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("antibs", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Get(),
                ],
                [null, null, "branford", null, "alps", null, "bradford", null, "bradford", null, "bradford", "orl"]
            },
            {
                // The same script with a seventh, top-scoring add in place of that
                // trailing bare get: "forest" takes rank 1, so the sixth query still
                // lands on "bradford" even though every earlier rank shifted down.
                [
                    RankTrackerOp.Add("bradford", 2),
                    RankTrackerOp.Add("branford", 3),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("alps", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("orl", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("orlando", 3),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("antibs", 2),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Add("forest", 4),
                    RankTrackerOp.Get(),
                ],
                [null, null, "branford", null, "alps", null, "bradford", null, "bradford", null, "bradford", null, "bradford"]
            },
            {
                // Strictly descending scores added up front, then drained: the ranks
                // come back in insertion order.
                [
                    RankTrackerOp.Add("first", 30),
                    RankTrackerOp.Add("second", 20),
                    RankTrackerOp.Add("third", 10),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Get(),
                    RankTrackerOp.Get(),
                ],
                [null, null, null, "first", "second", "third"]
            },
            {
                // Equal scores: the lexicographically smaller name outranks, even
                // though it was added second.
                [RankTrackerOp.Add("b", 1), RankTrackerOp.Add("a", 1), RankTrackerOp.Get(), RankTrackerOp.Get()],
                [null, null, "a", "b"]
            },
            {
                [RankTrackerOp.Add("solo", 5), RankTrackerOp.Get()],
                [null, "solo"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByResortEveryGet_LeetCodeExamples_ReturnsSuccessiveRanks(
        RankTrackerOp[] operations, string?[] expected) =>
        Assert.Equal(expected, RunScript(SequentiallyOrdinalRankTrackerSolution.CreateByResortEveryGet(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTwoHeaps_LeetCodeExamples_ReturnsSuccessiveRanks(
        RankTrackerOp[] operations, string?[] expected) =>
        Assert.Equal(expected, RunScript(SequentiallyOrdinalRankTrackerSolution.CreateByTwoHeaps(), operations));

    private static string?[] RunScript(
        SequentiallyOrdinalRankTrackerSolution.IRankTracker tracker,
        RankTrackerOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(tracker))];

    // One call in a rank-tracker script: which method to invoke and with what
    // arguments. Pure dispatch, built via the named factories below so a script (like
    // Examples above) reads like the LeetCode call sequence it replays. Add returns
    // null (no comparable value); Get returns the actual answer - the same
    // null-means-no-return-value convention LRUCacheOp.Apply uses for its own put/get
    // split. Nested here rather than left at file scope so the file declares exactly
    // one type.
    public readonly record struct RankTrackerOp(RankTrackerOp.OpKind kind, string name, int score)
    {
        public static RankTrackerOp Add(string name, int score) => new(OpKind.Add, name, score);

        public static RankTrackerOp Get() => new(OpKind.Get, "", 0);

        // Internal, not public: only this same assembly's test method ever calls Apply.
        internal string? Apply(SequentiallyOrdinalRankTrackerSolution.IRankTracker tracker)
        {
            if (kind == OpKind.Add)
            {
                tracker.Add(name, score);
                return null;
            }

            return tracker.Get();
        }

        public enum OpKind
        {
            Add,
            Get,
        }
    }
}
