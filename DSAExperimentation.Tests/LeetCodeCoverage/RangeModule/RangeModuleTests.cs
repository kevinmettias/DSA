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
        AssertQueryRange(module, 10, 14, expectedCovered: true);
        AssertQueryRange(module, 13, 15, expectedCovered: true);
        AssertQueryRange(module, 16, 17, expectedCovered: true);

        module.RemoveRange(14, 16);
        AssertQueryRange(module, 10, 14, expectedCovered: true);
        AssertQueryRange(module, 13, 15, expectedCovered: false);
        AssertQueryRange(module, 16, 17, expectedCovered: true);
    }

    [Fact]
    public void QueryRange_SpansTwoUnmergedRanges_ReturnsFalse()
    {
        var module = new RangeModule();

        module.AddRange(1, 3);
        module.AddRange(5, 7);

        AssertQueryRange(module, 2, 6, expectedCovered: false);
    }

    [Fact]
    public void AddRange_TouchingRanges_MergeIntoOneContiguousRange()
    {
        var module = new RangeModule();

        module.AddRange(1, 3);
        module.AddRange(3, 5);

        AssertQueryRange(module, 1, 5, expectedCovered: true);
    }

    [Fact]
    public void RemoveRange_EntireTrackedRange_LeavesNothingQueryable()
    {
        var module = new RangeModule();

        module.AddRange(1, 10);
        module.RemoveRange(1, 10);

        AssertQueryRange(module, 1, 10, expectedCovered: false);
        AssertQueryRange(module, 2, 3, expectedCovered: false);
    }

    private static void AssertQueryRange(RangeModule module, int left, int right, bool expectedCovered)
    {
        var actual = module.QueryRange(left, right);

        if (expectedCovered)
        {
            Assert.True(actual);
        }
        else
        {
            Assert.False(actual);
        }
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
                AddSurvivingFragments(_ranges.Get(i), (left, right), survivors);
            }

            _ranges = new IntervalSet<int>();
            foreach (var (start, end) in survivors)
            {
                _ranges.Add(start, end);
            }
        }

        private static void AddSurvivingFragments(
            (int Start, int End) interval,
            (int Left, int Right) removalRange,
            List<(int Start, int End)> survivors)
        {
            var (start, end) = interval;
            var (left, right) = removalRange;

            if (end <= left || start >= right)
            {
                survivors.Add((start, end));
                return;
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

        private readonly struct StartsView(IntervalSet<int> intervals) : IRandomAccessSequence<int>
        {
            public int Length => intervals.Count;

            public int Get(int index) => intervals.Get(index).Start;
        }
    }
}
