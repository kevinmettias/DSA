using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.HeightChecker;

// LeetCode 1051. Height Checker: the "expected" line-up is the heights sorted
// non-decreasingly, and the answer is how many positions differ between the
// original order and that one.
//
// Both strategies sort a copy and then count mismatches; they differ only in the
// sort - the textbook O(n^2) insertion sort a baseline would hand-roll, versus this
// repo's MergeSort over ArrayIndexedSequence.
internal static class HeightCheckerSolution
{
    // Baseline: insertion sort of a copy, BCL-only internals.
    public static int CountMismatchesByInsertionSort(int[] heights)
    {
        var expected = heights.ToArray();

        for (var i = 1; i < expected.Length; i++)
        {
            InsertOne(expected, i);
        }

        return CountMismatches(heights, expected);
    }

    private static void InsertOne(int[] expected, int index)
    {
        var current = expected[index];
        var j = index - 1;

        while (j >= 0 && expected[j] > current)
        {
            expected[j + 1] = expected[j];
            j--;
        }

        expected[j + 1] = current;
    }

    // Same shape, with this repo's MergeSort composed over ArrayIndexedSequence.
    public static int CountMismatchesByMergeSort(int[] heights)
    {
        var expected = heights.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(expected));

        return CountMismatches(heights, expected);
    }

    private static int CountMismatches(int[] heights, int[] expected)
    {
        var mismatches = 0;

        for (var i = 0; i < heights.Length; i++)
        {
            if (heights[i] != expected[i])
            {
                mismatches++;
            }
        }

        return mismatches;
    }
}
