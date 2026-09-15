using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.WiggleSortII;

// LeetCode 324. Wiggle Sort II: rearrange nums in place so nums[0] < nums[1] >
// nums[2] < nums[3]... Both strategies sort a clone of the input, then fill the
// even/odd index positions from the reversed lower and upper halves of that sorted
// clone - filling from the *reversed* halves, rather than a naive ascending
// interleave, is what keeps the result valid even when values repeat around the
// median. The two strategies differ only in which sort produces the clone.
internal static class WiggleSortIISolution
{
    // The textbook O(n^2) selection sort, written without this repo's primitives -
    // the arm the composed solution below has to justify itself against.
    public static void WiggleSortBySelectionSort(int[] nums)
    {
        var sorted = (int[])nums.Clone();

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

        InterleaveReversedHalves(sorted, nums);
    }

    // This repo's own O(n log n) MergeSort over ArrayIndexedSequence (HIndex
    // precedent) in place of the textbook sort.
    public static void WiggleSortByMergeSort(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        InterleaveReversedHalves(sorted, nums);
    }

    // Shared by both strategies so the only thing they differ in is the sort that
    // produces `sorted`. Reads the lower half back-to-front into the even indices
    // and the upper half back-to-front into the odd indices.
    private static void InterleaveReversedHalves(int[] sorted, int[] destination)
    {
        var n = destination.Length;
        var lowIndex = (n - 1) / 2;
        var highIndex = n - 1;

        // One of these two runs per index - the cursor it reads is the one it
        // advances, so exactly one half is consumed per position.
        int NextLowerValue() => sorted[lowIndex--];

        int NextUpperValue() => sorted[highIndex--];

        for (var i = 0; i < n; i++)
        {
            var isEvenIndex = i % 2 == 0;
            destination[i] = isEvenIndex ? NextLowerValue() : NextUpperValue();
        }
    }
}
