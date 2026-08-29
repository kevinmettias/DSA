using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeIntervals;

// LeetCode 56. Merge Intervals: this repo's own IntervalSet<TKey> already maintains
// a merged, sorted set of disjoint intervals as each one is added - the same
// closed-interval semantics LC 56 asks for (see IntervalSet.cs's own doc comment).
public sealed partial class MergeIntervalsTests
{
    [Fact]
    public void Add_OverlappingIntervals_MergesIntoDisjointRanges()
    {
        var intervals = new IntervalSet<int>();

        intervals.Add(1, 3);
        intervals.Add(2, 6);
        intervals.Add(8, 10);
        intervals.Add(15, 18);

        Assert.Equal(3, intervals.Count);
        Assert.Equal((1, 6), intervals.Get(0));
        Assert.Equal((8, 10), intervals.Get(1));
        Assert.Equal((15, 18), intervals.Get(2));
    }
}
