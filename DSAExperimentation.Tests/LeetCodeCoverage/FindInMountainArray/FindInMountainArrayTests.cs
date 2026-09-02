using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindInMountainArray;

// LeetCode 1095. Find in Mountain Array: a hand-rolled bisection over this repo's own
// SearchRange locates the peak (the one part with no existing primitive to compose -
// every solution to this problem needs a bespoke peak search), then the ascending and
// descending slopes are handed to BinarySearch.Find as two ordinary sorted ranges -
// the descending slope via a reversed IComparer<int>, exactly the "any total order is
// a valid comparer" contract BinarySearch.Find's own doc comment relies on
// (ARCHITECTURE.md Sec 9.2). OffsetSequence is a thin IRandomAccessSequence<int>
// witness over a sub-range of the backing array, so neither slope needs to be copied.
public sealed partial class FindInMountainArrayTests
{
    private static readonly IComparer<int> Descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

    [Fact]
    public void SearchMountainArray_TargetOnAscendingSlope_ReturnsItsIndex()
    {
        int[] mountain = [1, 2, 3, 4, 5, 3, 1];

        var index = SearchMountainArray(mountain, target: 3);

        Assert.Equal(2, index);
    }

    [Fact]
    public void SearchMountainArray_TargetOnDescendingSlopeOnly_ReturnsItsIndex()
    {
        int[] mountain = [1, 5, 10, 20, 15, 8, 2];

        var index = SearchMountainArray(mountain, target: 8);

        Assert.Equal(5, index);
    }

    [Fact]
    public void SearchMountainArray_TargetAbsent_ReturnsNegativeOne()
    {
        int[] mountain = [0, 1, 2, 4, 2, 1];

        var index = SearchMountainArray(mountain, target: 3);

        Assert.Equal(-1, index);
    }

    private static int SearchMountainArray(int[] mountain, int target)
    {
        var peakIndex = FindPeakIndex(mountain);

        var ascending = new OffsetSequence(mountain, offset: 0, peakIndex + 1);
        var ascendingIndex = BinarySearch.Find(ascending, target);
        if (ascendingIndex is not null)
        {
            return ascendingIndex.Value;
        }

        var descending = new OffsetSequence(mountain, peakIndex, mountain.Length - peakIndex);
        var descendingIndex = BinarySearch.Find(descending, target, Descending);

        return descendingIndex is null ? -1 : descendingIndex.Value + peakIndex;
    }

    private static int FindPeakIndex(int[] mountain)
    {
        var sequence = new OffsetSequence(mountain, offset: 0, mountain.Length);
        var range = new SearchRange(0, mountain.Length - 1);

        while (range.Low < range.High)
        {
            var mid = range.Low + ((range.High - range.Low) / AlgorithmConstants.HalvingFactor);

            range = sequence.Get(mid) < sequence.Get(mid + 1)
                ? range with { Low = mid + 1 }
                : range with { High = mid };
        }

        return range.Low;
    }

    private readonly struct OffsetSequence(int[] items, int offset, int length) : IRandomAccessSequence<int>
    {
        public int Length { get; } = length;

        public int Get(int index) => items[offset + index];
    }
}
