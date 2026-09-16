using DSAExperimentation.LeetCode.DataStreamAsDisjointIntervals;

namespace DSAExperimentation.LeetCode.Tests.DataStreamAsDisjointIntervals;

// The seam between DataStreamAsDisjointIntervalsSolution's two summarizers.
// CreateByFullRebuildEachCall keeps every raw value and re-derives the summary by
// sorting and rescanning. CreateByIntervalSetMerge composes this repo's own
// DataStructures.IntervalSet<int> under the width-1 half-open encoding: value v is
// stored as the pair (v, v + 1), so two originally-adjacent values v and v + 1 become
// (v, v+1) and (v+1, v+2), which share the boundary value v + 1 and merge under
// IntervalSet's own closed-interval overlap rule. GetIntervals reverses the encoding
// by reporting (Start, End - 1).
//
// The encoding is the entire contract, and it is a lifecycle contract: whether two
// values merge depends on what was stored before them, so the summary after a
// stream is not derivable from any single AddNum. Each test below runs a stream
// against both arms and compares the summaries they report after every step.
public sealed partial class IntervalSetSummarySeamTests
{
    [Fact]
    public void GetIntervals_LeetCodeExample_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(1),
            StreamOperation.Summarize(),
            StreamOperation.Add(3),
            StreamOperation.Summarize(),
            StreamOperation.Add(7),
            StreamOperation.Summarize(),
            StreamOperation.Add(2),
            StreamOperation.Summarize(),
            StreamOperation.Add(6),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(
            ["[1,1]", "[1,1] [3,3]", "[1,1] [3,3] [7,7]", "[1,3] [7,7]", "[1,3] [6,7]"],
            Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    // The values that make the encoding earn itself: consecutive integers have to
    // merge, and integers with one whole number missing between them must not.
    [Fact]
    public void GetIntervals_AdjacentAndNearlyAdjacentValues_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(1),
            StreamOperation.Add(2),
            StreamOperation.Summarize(),
            StreamOperation.Add(4),
            StreamOperation.Summarize(),
            StreamOperation.Add(3),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(["[1,2]", "[1,2] [4,4]", "[1,4]"], Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    // A value that bridges two existing runs: one add has to close two gaps at once.
    [Fact]
    public void AddNum_ValueBridgingTwoRuns_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(1),
            StreamOperation.Add(3),
            StreamOperation.Add(5),
            StreamOperation.Add(4),
            StreamOperation.Summarize(),
            StreamOperation.Add(2),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(
            ["[1,1] [3,5]", "[1,5]"],
            Replay(script, useIntervalSet: true).Where(summary => summary.Length > 0));
        AssertSameSummaries(script);
    }

    [Fact]
    public void AddNum_DuplicateValue_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(7),
            StreamOperation.Add(7),
            StreamOperation.Add(7),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(["[7,7]"], Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    [Fact]
    public void GetIntervals_OnAnEmptyStream_MatchesFullRebuild()
    {
        StreamOperation[] script = [StreamOperation.Summarize(), StreamOperation.Summarize()];

        Assert.Equal(["", ""], Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    // Values arriving in descending order: every add lands at the low end of what is
    // already stored, so the merge always reaches backwards through the set.
    [Fact]
    public void AddNum_DescendingValues_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(10),
            StreamOperation.Add(9),
            StreamOperation.Add(8),
            StreamOperation.Summarize(),
            StreamOperation.Add(6),
            StreamOperation.Add(7),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(["[8,10]", "[6,10]"], Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    [Fact]
    public void GetIntervals_NegativeAndZeroValues_MatchesFullRebuild()
    {
        StreamOperation[] script =
        [
            StreamOperation.Add(0),
            StreamOperation.Add(-1),
            StreamOperation.Add(-5),
            StreamOperation.Summarize(),
        ];

        Assert.Equal(["[-5,-5] [-1,0]"], Replay(script, useIntervalSet: true));
        AssertSameSummaries(script);
    }

    private static void AssertSameSummaries(StreamOperation[] script)
        => Assert.Equal(Replay(script, useIntervalSet: false), Replay(script, useIntervalSet: true));

    private static List<string> Replay(StreamOperation[] script, bool useIntervalSet)
    {
        var ranges = useIntervalSet
            ? DataStreamAsDisjointIntervalsSolution.CreateByIntervalSetMerge()
            : DataStreamAsDisjointIntervalsSolution.CreateByFullRebuildEachCall();
        var transcript = new List<string>();

        foreach (var operation in script)
        {
            if (operation.IsAdd)
            {
                ranges.AddNum(operation.Value);
                continue;
            }

            transcript.Add(string.Join(' ', ranges.GetIntervals().Select(interval => $"[{interval.Start},{interval.End}]")));
        }

        return transcript;
    }

    private readonly record struct StreamOperation(bool IsAdd, int Value)
    {
        public static StreamOperation Add(int value) => new(IsAdd: true, value);

        public static StreamOperation Summarize() => new(IsAdd: false, Value: 0);
    }
}
