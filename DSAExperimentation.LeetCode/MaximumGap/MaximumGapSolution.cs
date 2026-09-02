using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumGap;

// LeetCode 164. Maximum Gap: the largest gap between successive elements once
// nums is sorted. The two strategies differ only in how they sort a copy of
// nums - a textbook O(n^2) selection sort or this repo's own O(n log n)
// MergeSort over ArrayIndexedSequence - then share the same linear scan for the
// largest adjacent gap.
internal static class MaximumGapSolution
{
    // The textbook baseline: BCL selection sort, written without this repo's
    // primitives - the arm the composed MergeSort strategy below has to justify
    // itself against.
    public static int MaximumGapBySelectionSort(int[] nums)
    {
        if (nums.Length < 2)
        {
            return 0;
        }

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

        return MaxAdjacentGap(sorted);
    }

    // Sort a copy of nums with this repo's own MergeSort over
    // ArrayIndexedSequence, then the same adjacent-gap scan.
    public static int MaximumGapByMergeSort(int[] nums)
    {
        if (nums.Length < 2)
        {
            return 0;
        }

        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return MaxAdjacentGap(sorted);
    }

    private static int MaxAdjacentGap(int[] sorted)
    {
        var maxGap = 0;

        for (var i = 1; i < sorted.Length; i++)
        {
            maxGap = Math.Max(maxGap, sorted[i] - sorted[i - 1]);
        }

        return maxGap;
    }
}
