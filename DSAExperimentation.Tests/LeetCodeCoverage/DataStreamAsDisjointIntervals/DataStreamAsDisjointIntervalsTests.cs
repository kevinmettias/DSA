using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DataStreamAsDisjointIntervals;

// LeetCode 352. Data Stream as Disjoint Intervals: addNum needs adjacent integers (a gap of
// exactly 1, e.g. 1 and 2) to merge into one summarized range - a stronger "touching" rule
// than IntervalSet<TKey>'s own closed-interval-overlap merge, which only merges intervals
// that share an actual boundary VALUE (see IntervalSet.cs's own doc comment: it deliberately
// carries no TKey increment operation to express "adjacent" for an arbitrary TKey). Because
// TKey is concretely int here, the call site - not IntervalSet itself - can still get
// adjacency-merging for free by encoding each seen value v as the width-1 half-open range
// [v, v+1), stored as the closed pair (v, v+1): two originally-adjacent values v and v+1 then
// become (v, v+1) and (v+1, v+2), which DO share the boundary value v+1 and merge under
// IntervalSet's existing overlap rule. GetIntervals reverses the encoding by reporting
// (Start, End-1).
public sealed partial class DataStreamAsDisjointIntervalsTests
{
    [Fact]
    public void AddNum_LeetCodeExample_SummarizesAsDisjointIntervals()
    {
        var stream = new SummaryRanges();

        stream.AddNum(1);
        Assert.Equal([(1, 1)], stream.GetIntervals());

        stream.AddNum(3);
        Assert.Equal([(1, 1), (3, 3)], stream.GetIntervals());

        stream.AddNum(7);
        Assert.Equal([(1, 1), (3, 3), (7, 7)], stream.GetIntervals());

        stream.AddNum(2);
        Assert.Equal([(1, 3), (7, 7)], stream.GetIntervals());

        stream.AddNum(6);
        Assert.Equal([(1, 3), (6, 7)], stream.GetIntervals());
    }

    [Fact]
    public void AddNum_DuplicateValue_LeavesIntervalsUnchanged()
    {
        var stream = new SummaryRanges();

        stream.AddNum(5);
        stream.AddNum(5);

        Assert.Equal([(5, 5)], stream.GetIntervals());
    }

    private sealed class SummaryRanges
    {
        private readonly IntervalSet<int> _intervals = new();

        public void AddNum(int value) => _intervals.Add(value, value + 1);

        public List<(int Start, int End)> GetIntervals()
            => Enumerable.Range(0, _intervals.Count)
                .Select(_intervals.Get)
                .Select(interval => (interval.Start, interval.End - 1))
                .ToList();
    }
}
