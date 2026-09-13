using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SortAnArray;

// LeetCode 912. Sort an Array: return nums in ascending order, without using the
// language's built-in sort.
//
// The problem *is* a comparison sort, so the two strategies are the two sorts
// themselves - a textbook O(n^2) insertion sort, and this repo's own
// Algorithms.Sorting.MergeSort over the array wrapped as an IIndexedSequence<int>,
// which needs no glue beyond the wrapping. Both sort a copy rather than nums
// itself, so a caller (and a benchmark re-invoking an arm) always starts from the
// same unsorted input.
internal static class SortAnArraySolution
{
    // The textbook baseline: BCL insertion sort over a plain array, written
    // without this repo's primitives - the arm the composed MergeSort strategy
    // below has to justify itself against.
    public static int[] SortArrayByInsertionSort(int[] nums)
    {
        var sorted = nums.ToArray();

        for (var i = 1; i < sorted.Length; i++)
        {
            var current = sorted[i];
            var j = i - 1;

            while (j >= 0 && sorted[j] > current)
            {
                sorted[j + 1] = sorted[j];
                j--;
            }

            sorted[j + 1] = current;
        }

        return sorted;
    }

    // This repo's own O(n log n) MergeSort, which sorts anything satisfying
    // IIndexedSequence<Element> in place - so an int[] wrapped in
    // ArrayIndexedSequence<int> is the whole solution.
    public static int[] SortArrayByMergeSort(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return sorted;
    }
}
