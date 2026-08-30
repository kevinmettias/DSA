using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.IntervalSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeModule;

// LeetCode 715. Range Module: half-open [left, right) ranges tracked over the real
// line. addRange composes straight onto IntervalSet<int>.Add - unlike
// DataStreamAsDisjointIntervalsTests (LC 352), no +1 encoding trick is needed here,
// because IntervalSet's closed-interval overlap rule (a <= d && c <= b) already
// treats two half-open ranges stored as raw (Start, End) pairs as touching/merging
// exactly when they share a boundary value, which is precisely the "no gap between
// them" condition half-open range coverage needs. queryRange finds the one interval
// that could contain [left, right) via BinarySearch.UpperBound over a Starts view -
// the same "IRandomAccessSequence witness over an existing structure's own Get" idiom
// IntervalSet.cs's own HasOverlap/Add already use internally (its own doc comment
// names this as a sanctioned reuse, ARCHITECTURE.md §4's ByPriorityOrder precedent) -
// then checks that candidate's End reaches right. removeRange has no counterpart on
// IntervalSet (it exposes no Remove), so it rebuilds: split every stored interval
// that overlaps [left, right) into its surviving fragments and re-Add each one to a
// fresh IntervalSet, composing only Get/Count/Add, never reaching into IntervalSet's
// private storage.
public sealed partial class RangeModuleTests
{
    [Fact]
    public void RangeModule_LeetCodeExample_TracksAddQueryAndRemove()
    {
        var module = new RangeModule();

        module.AddRange(10, 20);
        Assert.True(module.QueryRange(10, 14));
        Assert.True(module.QueryRange(13, 15));
        Assert.True(module.QueryRange(16, 17));

        module.RemoveRange(14, 16);
        Assert.True(module.QueryRange(10, 14));
        Assert.False(module.QueryRange(13, 15));
        Assert.True(module.QueryRange(16, 17));
    }

    [Fact]
    public void QueryRange_SpansTwoUnmergedRanges_ReturnsFalse()
    {
        var module = new RangeModule();

        module.AddRange(1, 3);
        module.AddRange(5, 7);

        Assert.False(module.QueryRange(2, 6));
    }

    [Fact]
    public void AddRange_TouchingRanges_MergeIntoOneContiguousRange()
    {
        var module = new RangeModule();

        module.AddRange(1, 3);
        module.AddRange(3, 5);

        Assert.True(module.QueryRange(1, 5));
    }

    [Fact]
    public void RemoveRange_EntireTrackedRange_LeavesNothingQueryable()
    {
        var module = new RangeModule();

        module.AddRange(1, 10);
        module.RemoveRange(1, 10);

        Assert.False(module.QueryRange(1, 10));
        Assert.False(module.QueryRange(2, 3));
    }

    private sealed class RangeModule
    {
        private IntervalSet<int> _ranges = new();

        public void AddRange(int left, int right) => _ranges.Add(left, right);

        public bool QueryRange(int left, int right)
        {
            var candidate = BinarySearch.UpperBound<int, StartsView>(new StartsView(_ranges), left) - 1;
            return candidate >= 0 && _ranges.Get(candidate).End >= right;
        }

        public void RemoveRange(int left, int right)
        {
            var survivors = new List<(int Start, int End)>();

            for (var i = 0; i < _ranges.Count; i++)
            {
                var (start, end) = _ranges.Get(i);
                if (end <= left || start >= right)
                {
                    survivors.Add((start, end));
                    continue;
                }

                if (start < left)
                {
                    survivors.Add((start, left));
                }

                if (right < end)
                {
                    survivors.Add((right, end));
                }
            }

            _ranges = new IntervalSet<int>();
            foreach (var (start, end) in survivors)
            {
                _ranges.Add(start, end);
            }
        }

        private readonly struct StartsView(IntervalSet<int> intervals) : IRandomAccessSequence<int>
        {
            public int Length => intervals.Count;

            public int Get(int index) => intervals.Get(index).Start;
        }
    }
}
