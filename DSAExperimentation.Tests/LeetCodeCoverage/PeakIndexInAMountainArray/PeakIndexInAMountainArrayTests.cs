using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeakIndexInAMountainArray;

// LeetCode 852. Peak Index in a Mountain Array: the array is unimodal (strictly
// increasing then strictly decreasing), so encoding "is the step from i to i+1
// downhill" as a 0/1 sequence makes that sequence monotonic - false before the
// peak, true at the peak and after - and this repo's own BinarySearch.LowerBound
// finds the first true directly. Same proxy-sequence idiom
// FindMinimumInRotatedSortedArrayTests already uses for its own pivot search.
public sealed partial class PeakIndexInAMountainArrayTests
{
    [Theory]
    [InlineData(new[] { 0, 1, 0 }, 1)]
    [InlineData(new[] { 0, 2, 1, 0 }, 1)]
    [InlineData(new[] { 0, 10, 5, 2 }, 1)]
    [InlineData(new[] { 24, 69, 100, 99, 79, 78, 67, 36, 26, 19 }, 2)]
    public void PeakIndexOf_MountainArrayExamples_ReturnsPeakIndex(int[] mountain, int expected)
    {
        var peak = PeakIndexOf(mountain);

        Assert.Equal(expected, peak);
    }

    private static int PeakIndexOf(int[] mountain)
        => BinarySearch.LowerBound<int, DescendingStepSequence>(new DescendingStepSequence(mountain), 1);

    // Get(index) is 1 exactly when the step from mountain[index] to
    // mountain[index + 1] is downhill - the peak itself is the first such index,
    // and the unimodal precondition is what keeps this sequence monotonic, the
    // same unenforced-sortedness shape BinarySearch.LowerBound already leans on.
    private readonly struct DescendingStepSequence(int[] mountain) : IRandomAccessSequence<int>
    {
        public int Length => mountain.Length - 1;

        public int Get(int index) => mountain[index] > mountain[index + 1] ? 1 : 0;
    }
}
