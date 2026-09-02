using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountIntegersInIntervals;

// LeetCode 2276. Count Integers in Intervals: Add(left, right) delegates straight to this
// repo's own IntervalSet<int>.Add(left, right) - the closed-interval semantics IntervalSet
// already implements (see IntervalSet.cs's own doc comment) match this problem's "add every
// integer in [left, right]" exactly, no encoding needed: two ranges merge whenever they share an
// actual integer value (e.g. [1,3] and [3,5] both contain 3), which is precisely when NOT
// merging them would double-count that shared integer. Count() sums (End - Start + 1) over every
// disjoint interval IntervalSet currently holds instead of tracking a running total field, so it
// stays correct no matter how many times Add merges ranges behind it.
public sealed partial class CountIntegersInIntervalsTests
{
    [Fact]
    public void Add_LeetCodeExample_CountTracksUnionSizeAsRangesMerge()
    {
        var countIntervals = new CountIntervals();

        countIntervals.Add(2, 3);
        Assert.Equal(2, countIntervals.Count());

        countIntervals.Add(7, 10);
        Assert.Equal(6, countIntervals.Count());

        countIntervals.Add(5, 8);
        Assert.Equal(8, countIntervals.Count());
    }

    [Fact]
    public void Add_DisjointThenBridgingRange_MergesEverythingIntoOneUnion()
    {
        var countIntervals = new CountIntervals();

        countIntervals.Add(1, 3);
        countIntervals.Add(5, 7);
        Assert.Equal(6, countIntervals.Count());

        countIntervals.Add(3, 5);
        Assert.Equal(7, countIntervals.Count());
    }

    private sealed class CountIntervals
    {
        private readonly IntervalSet<int> _intervals = new();

        public void Add(int left, int right) => _intervals.Add(left, right);

        public int Count()
        {
            var total = 0;

            for (var i = 0; i < _intervals.Count; i++)
            {
                var (start, end) = _intervals.Get(i);
                total += end - start + 1;
            }

            return total;
        }
    }
}
