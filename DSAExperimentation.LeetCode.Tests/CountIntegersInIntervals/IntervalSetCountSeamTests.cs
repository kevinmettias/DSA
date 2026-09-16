using DSAExperimentation.LeetCode.CountIntegersInIntervals;

namespace DSAExperimentation.LeetCode.Tests.CountIntegersInIntervals;

// The seam between CountIntegersInIntervalsSolution's two counters.
// CreateByHashSetPerInteger walks every integer of an added range into a BCL
// HashSet<int>; CreateByIntervalSetMerge hands the same range to this repo's own
// DataStructures.IntervalSet<int> and sums each stored interval's width.
//
// The two agree only because IntervalSet's merge rule and this problem's overlap
// rule are the same rule: two added ranges must collapse exactly when they share an
// integer, because a shared integer counted twice is a count that is too large by
// one. That is a statement about a lifecycle - a range merges against whatever the
// set already holds - so every test below adds enough ranges to make the set
// merge, split and re-merge, and compares the count each arm reports.
public sealed partial class IntervalSetCountSeamTests
{
    [Fact]
    public void Count_OverlappingAndAdjacentAdds_MatchesIntegerSet()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Add(1, 3),
            IntervalOperation.Count(),
            IntervalOperation.Add(4, 7),
            IntervalOperation.Count(),
            IntervalOperation.Add(1, 10),
            IntervalOperation.Count(),
        ];

        Assert.Equal(["3", "7", "10"], Replay(script, useIntervalSet: true));
        AssertSameCounts(script);
    }

    // Ranges sharing exactly one integer: [1, 3] and [3, 5] both contain 3, so they
    // must become one interval of width 5 rather than two of width 3 each.
    [Fact]
    public void Count_RangesSharingOneBoundaryInteger_MergeIntoOneInterval()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Add(1, 3),
            IntervalOperation.Add(3, 5),
            IntervalOperation.Count(),
        ];

        Assert.Equal(["5"], Replay(script, useIntervalSet: true));
        AssertSameCounts(script);
    }

    // The opposite boundary: [1, 3] and [4, 7] share no integer at all, so they stay
    // apart and their counts add.
    [Fact]
    public void Count_RangesWithAGapBetweenThem_StayTwoIntervals()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Add(1, 3),
            IntervalOperation.Add(4, 7),
            IntervalOperation.Count(),
        ];

        Assert.Equal(["7"], Replay(script, useIntervalSet: true));
        AssertSameCounts(script);
    }

    // Adding the same range twice must not widen the set, and a single-integer range
    // is the narrowest add the set has to place.
    [Fact]
    public void Count_RepeatedAndSingleIntegerRanges_MatchesIntegerSet()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Add(5, 5),
            IntervalOperation.Count(),
            IntervalOperation.Add(5, 5),
            IntervalOperation.Count(),
            IntervalOperation.Add(0, 0),
            IntervalOperation.Count(),
        ];

        Assert.Equal(["1", "1", "2"], Replay(script, useIntervalSet: true));
        AssertSameCounts(script);
    }

    // A range that swallows several stored ones at once: the sum has to come from
    // the merged interval, not from a running total that still remembers the pieces.
    [Fact]
    public void Count_RangeSwallowingSeveralStoredIntervals_MatchesIntegerSet()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Add(1, 2),
            IntervalOperation.Add(5, 6),
            IntervalOperation.Add(9, 10),
            IntervalOperation.Count(),
            IntervalOperation.Add(0, 20),
            IntervalOperation.Count(),
            IntervalOperation.Add(21, 30),
            IntervalOperation.Count(),
        ];

        AssertSameCounts(script);
    }

    [Fact]
    public void Count_InterleavedAddsAndQueries_MatchesIntegerSet()
    {
        IntervalOperation[] script =
        [
            IntervalOperation.Count(),
            IntervalOperation.Add(10, 20),
            IntervalOperation.Add(20, 30),
            IntervalOperation.Count(),
            IntervalOperation.Add(15, 18),
            IntervalOperation.Count(),
            IntervalOperation.Add(31, 31),
            IntervalOperation.Count(),
        ];

        AssertSameCounts(script);
    }

    private static void AssertSameCounts(IntervalOperation[] script)
        => Assert.Equal(Replay(script, useIntervalSet: false), Replay(script, useIntervalSet: true));

    private static List<string> Replay(IntervalOperation[] script, bool useIntervalSet)
    {
        var intervals = useIntervalSet
            ? CountIntegersInIntervalsSolution.CreateByIntervalSetMerge()
            : CountIntegersInIntervalsSolution.CreateByHashSetPerInteger();
        var transcript = new List<string>();

        foreach (var operation in script)
        {
            if (operation.IsAdd)
            {
                intervals.Add(operation.Left, operation.Right);
                continue;
            }

            transcript.Add(intervals.Count().ToString());
        }

        return transcript;
    }

    private readonly record struct IntervalOperation(bool IsAdd, int Left, int Right)
    {
        public static IntervalOperation Add(int left, int right) => new(IsAdd: true, left, right);

        public static IntervalOperation Count() => new(IsAdd: false, Left: 0, Right: 0);
    }
}
