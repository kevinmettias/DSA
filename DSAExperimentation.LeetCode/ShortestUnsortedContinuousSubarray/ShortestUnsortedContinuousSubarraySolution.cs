using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ShortestUnsortedContinuousSubarray;

// LeetCode 581. Shortest Unsorted Continuous Subarray: find the shortest span
// that, if re-sorted, would leave the whole array sorted.
//
// Both strategies sort a copy of the array and then scan inward from both ends
// for the leftmost/rightmost index where the original and the sorted copy
// disagree - the span between them is exactly what needs re-sorting. They
// differ only in how the copy gets sorted.
internal static class ShortestUnsortedContinuousSubarraySolution
{
    // The textbook baseline: O(n^2) selection sort, written without this repo's
    // primitives - the arm the composed solution below has to beat.
    public static int FindUnsortedSubarrayBySelectionSortScan(int[] nums)
    {
        var sorted = nums.ToArray();

        for (var i = 0; i < sorted.Length - 1; i++)
        {
            var minIndex = i;
            for (var j = i + 1; j < sorted.Length; j++)
            {
                if (sorted[j] < sorted[minIndex])
                {
                    minIndex = j;
                }
            }

            (sorted[i], sorted[minIndex]) = (sorted[minIndex], sorted[i]);
        }

        return UnsortedSpanLength(nums, sorted);
    }

    // This repo's own O(n log n) MergeSort over ArrayIndexedSequence - the same
    // composition MaximumGapTests uses.
    public static int FindUnsortedSubarrayByMergeSortScan(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return UnsortedSpanLength(nums, sorted);
    }

    private static int UnsortedSpanLength(int[] nums, int[] sorted)
    {
        var left = 0;
        while (left < nums.Length && nums[left] == sorted[left])
        {
            left++;
        }

        if (left == nums.Length)
        {
            return 0;
        }

        var right = nums.Length - 1;
        while (nums[right] == sorted[right])
        {
            right--;
        }

        return right - left + 1;
    }
}
