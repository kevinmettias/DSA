using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestSubarrayToBeRemovedToMakeArraySorted;

// LeetCode 1574. Shortest Subarray to be Removed to Make Array Sorted: a two-pointer
// pass finds the longest already-sorted prefix ([0, left]) and suffix ([right, n-1])
// in O(n) - plain array scanning, no primitive needed for that half. What DOES compose
// this repo's own primitives is stitching the two runs together: for every prefix index
// i, this repo's own BinarySearch.LowerBound over an ArraySequence<int> witness wrapping
// the (already-sorted) suffix slice finds the smallest suffix value >= arr[i] in
// O(log n) instead of a second nested scan - the same "search on a derived monotonic
// sequence" idiom MinimumSizeSubarraySumTests/ReversePairsTests already establish, here
// applied to the sorted suffix itself rather than a prefix-sum/coordinate-compressed
// derivation of it.
public sealed partial class ShortestSubarrayToBeRemovedToMakeArraySortedTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 10, 4, 2, 3, 5 }, 3)]
    [InlineData(new[] { 5, 4, 3, 2, 1 }, 4)]
    [InlineData(new[] { 1, 2, 3 }, 0)]
    [InlineData(new[] { 1 }, 0)]
    public void FindLengthOfShortestSubarray_LeetCodeExamples_ReturnsMinRemovalLength(int[] arr, int expected)
        => Assert.Equal(expected, FindLengthOfShortestSubarray(arr));

    private static int FindLengthOfShortestSubarray(int[] arr)
    {
        var n = arr.Length;

        var left = 0;
        while (left + 1 < n && arr[left] <= arr[left + 1])
        {
            left++;
        }

        if (left == n - 1)
        {
            return 0;
        }

        var right = n - 1;
        while (right > 0 && arr[right - 1] <= arr[right])
        {
            right--;
        }

        // Removing everything after the sorted prefix, or everything before the
        // sorted suffix, are always valid - the floor every stitched candidate below
        // has to beat.
        var best = Math.Min(n - left - 1, right);

        var suffix = new ArraySequence<int>(arr[right..]);
        for (var i = 0; i <= left; i++)
        {
            var j = right + BinarySearch.LowerBound(suffix, arr[i]);
            best = Math.Min(best, j - i - 1);
        }

        return best;
    }
}
