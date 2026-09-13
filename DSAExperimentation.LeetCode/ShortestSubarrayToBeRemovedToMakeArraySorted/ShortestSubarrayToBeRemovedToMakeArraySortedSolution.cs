using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ShortestSubarrayToBeRemovedToMakeArraySorted;

// LeetCode 1574. Shortest Subarray to be Removed to Make Array Sorted: remove one
// contiguous subarray so that what remains is non-decreasing, and report the
// shortest such removal.
//
// The two strategies answer the same question at opposite ends of the cost curve:
// test every (start, end) removal range by re-scanning the merged remainder, or find
// the longest already-sorted prefix and suffix once and stitch them.
internal static class ShortestSubarrayToBeRemovedToMakeArraySortedSolution
{
    // The textbook answer: every removal range, each validated by a full rescan of
    // the merged prefix + suffix - O(n^3), BCL-only, the arm the composed solution
    // below has to justify itself against.
    public static int FindLengthOfShortestSubarrayByBruteForce(int[] arr)
    {
        var n = arr.Length;
        var best = n;

        for (var start = 0; start <= n; start++)
        {
            for (var end = start; end <= n; end++)
            {
                if (IsMergedSorted(arr, start, end))
                {
                    best = Math.Min(best, end - start);
                }
            }
        }

        return best;
    }

    private static bool IsMergedSorted(int[] arr, int removeStart, int removeEnd)
    {
        var previous = int.MinValue;

        if (!IsNonDecreasingFrom(arr, 0, removeStart, ref previous))
        {
            return false;
        }

        return IsNonDecreasingFrom(arr, removeEnd, arr.Length, ref previous);
    }

    private static bool IsNonDecreasingFrom(int[] arr, int start, int end, ref int previous)
    {
        for (var i = start; i < end; i++)
        {
            if (arr[i] < previous)
            {
                return false;
            }

            previous = arr[i];
        }

        return true;
    }

    // A two-pointer pass finds the longest already-sorted prefix ([0, left]) and
    // suffix ([right, n-1]) in O(n) - plain array scanning, no primitive needed for
    // that half. What DOES compose this repo's own primitives is stitching the two
    // runs together: for every prefix index i, this repo's own BinarySearch.LowerBound
    // over an ArraySequence<int> witness wrapping the (already-sorted) suffix slice
    // finds the smallest suffix value >= arr[i] in O(log n) instead of a second
    // nested scan - the same "search on a derived monotonic sequence" idiom
    // MinimumSizeSubarraySum and ReversePairs already establish, here applied to the
    // sorted suffix itself rather than a prefix-sum or coordinate-compressed
    // derivation of it.
    public static int FindLengthOfShortestSubarrayByBinarySearchStitch(int[] arr)
    {
        var left = FindSortedPrefixEnd(arr);

        if (left == arr.Length - 1)
        {
            return 0;
        }

        var right = FindSortedSuffixStart(arr);

        return StitchShortestRemoval(arr, left, right);
    }

    private static int FindSortedPrefixEnd(int[] arr)
    {
        var left = 0;

        while (left + 1 < arr.Length && arr[left] <= arr[left + 1])
        {
            left++;
        }

        return left;
    }

    private static int FindSortedSuffixStart(int[] arr)
    {
        var right = arr.Length - 1;

        while (right > 0 && arr[right - 1] <= arr[right])
        {
            right--;
        }

        return right;
    }

    // Removing everything after the sorted prefix, or everything before the sorted
    // suffix, are always valid - the floor every stitched candidate below has to
    // beat. Then binary-searches, for every prefix index, the smallest suffix value
    // >= arr[i] to find a shorter stitched removal.
    private static int StitchShortestRemoval(int[] arr, int left, int right)
    {
        var n = arr.Length;
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
