using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumMedianSumOfSubsequencesOfSizeThree;

// LeetCode 3627. Maximum Median Sum of Subsequences of Size 3: repeatedly pick
// any three remaining elements, take their median, remove them, until the
// array is empty; maximize the sum of medians collected. Both strategies
// answer the same question with the same signature (TwoSumSolution
// precedent).
internal static class MaximumMedianSumOfSubsequencesOfSizeThreeSolution
{
    // Textbook baseline: try every way to partition the array into groups of
    // three, recursing on what is left after each choice and taking the best
    // total - correct, and the arm the sorted-greedy strategy below has to
    // beat, but its search tree blows up combinatorially past a dozen or so
    // elements, unusable at the real problem's n up to 5*10^5.
    public static long MaximumMedianSumByBruteForce(int[] nums)
    {
        var used = new bool[nums.Length];

        return BestMedianSum(nums, used, nums.Length);
    }

    private static long BestMedianSum(int[] nums, bool[] used, int remainingCount)
    {
        if (remainingCount == 0)
        {
            return 0;
        }

        var best = long.MinValue;

        for (var a = 0; a < nums.Length; a++)
        {
            if (used[a])
            {
                continue;
            }

            for (var b = a + 1; b < nums.Length; b++)
            {
                if (used[b])
                {
                    continue;
                }

                for (var c = b + 1; c < nums.Length; c++)
                {
                    if (used[c])
                    {
                        continue;
                    }

                    used[a] = used[b] = used[c] = true;
                    var candidate = MedianOfThree(nums[a], nums[b], nums[c]) +
                        BestMedianSum(nums, used, remainingCount - 3);
                    used[a] = used[b] = used[c] = false;

                    best = Math.Max(best, candidate);
                }
            }
        }

        return best;
    }

    private static int MedianOfThree(int x, int y, int z) =>
        x + y + z - Math.Max(x, Math.Max(y, z)) - Math.Min(x, Math.Min(y, z));

    // Sorted ascending, the largest third of the array holds every eventual
    // median: pairing each of those with its two largest still-unclaimed
    // neighbors below it (one to its immediate left, the smallest of the
    // bottom two-thirds still spared for a later median) wastes nothing on
    // values too small to ever win a median contest. This repo's own
    // MergeSort.Sort<Element, TSequence> over an ArrayIndexedSequence (the
    // MatrixCellsInDistanceOrderBenchmarks precedent) does the ordering; the
    // greedy sum itself is then one linear pass, every other element from the
    // n/3 mark onward.
    public static long MaximumMedianSumBySortedGreedy(int[] nums)
    {
        var sorted = (int[])nums.Clone();

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0L;

        for (var i = sorted.Length / 3; i < sorted.Length; i += 2)
        {
            sum += sorted[i];
        }

        return sum;
    }
}
