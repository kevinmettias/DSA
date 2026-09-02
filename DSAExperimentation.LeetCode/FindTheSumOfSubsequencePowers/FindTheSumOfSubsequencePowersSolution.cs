using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheSumOfSubsequencePowers;

// LeetCode 3098. Find the Sum of Subsequence Powers: sum, over every length-k
// subsequence of nums, the minimum absolute difference between any two of its
// elements ("power"), mod 1e9+7.
//
// Sorted, a subsequence's power is just the minimum GAP between its consecutively
// chosen elements. The composed strategy leans on the identity
// power(S) = sum_{v=1}^{maxGap} [every adjacent gap in S is >= v]: summing that over
// every length-k subsequence swaps the order to
// sum_v (count of length-k subsequences whose adjacent gaps are all >= v), and that
// inner count is only a step function of v, changing at the handful of actual
// pairwise gaps present in nums - so it is enough to evaluate it once per distinct
// gap and weight by how wide a run of v it covers.
internal static class FindTheSumOfSubsequencePowersSolution
{
    // The textbook answer: enumerate every length-k subsequence directly (2^n
    // branch/skip choices), tracking the running minimum gap as it is built.
    // Deliberately plain recursion, no memoization and no repo primitives - the arm
    // the threshold-counting strategy below has to justify itself against.
    public static int SumOfPowersByBruteForce(int[] nums, int k)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);

        var total = SumPowers(sorted, k, index: 0, picked: 0, lastValue: 0, minGap: long.MaxValue);

        return (int)(total % ModularArithmetic.Modulo);
    }

    private static long SumPowers(int[] sorted, int k, int index, int picked, int lastValue, long minGap)
    {
        if (picked == k)
        {
            return minGap;
        }

        if (index == sorted.Length)
        {
            return 0;
        }

        var nextMinGap = picked == 0 ? long.MaxValue : Math.Min(minGap, sorted[index] - lastValue);
        var take = SumPowers(sorted, k, index + 1, picked + 1, sorted[index], nextMinGap);
        var skip = SumPowers(sorted, k, index + 1, picked, lastValue, minGap);

        return take + skip;
    }

    // Reuses this repo's own Sort (MergeSort over an ArrayIndexedSequence) for both
    // sorts it needs, and Algorithms.DynamicProgramming.Memoizer for the per-threshold
    // "how many length-k subsequences survive this minimum gap" count, and
    // Domain.Modular.ModularArithmetic for LeetCode's own 1e9+7 reporting convention.
    public static int SumOfPowersByThresholdCounting(int[] nums, int k)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var gaps = PairwiseGaps(sorted);
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(gaps));

        var total = 0L;
        var previous = 0;

        foreach (var threshold in gaps)
        {
            var width = threshold - previous;

            if (width > 0)
            {
                var count = CountAtLeast(sorted, k, threshold) % ModularArithmetic.Modulo;
                total = (total + width * count) % ModularArithmetic.Modulo;
            }

            previous = threshold;
        }

        return (int)total;
    }

    private static int[] PairwiseGaps(int[] sorted)
    {
        var gaps = new int[sorted.Length * (sorted.Length - 1) / 2];
        var next = 0;

        for (var i = 0; i < sorted.Length; i++)
        {
            for (var j = i + 1; j < sorted.Length; j++)
            {
                gaps[next++] = sorted[j] - sorted[i];
            }
        }

        return gaps;
    }

    // State (Last, Remaining): Last = -1 means "nothing chosen yet" (any element may
    // start the chain); Remaining counts elements still needed. Every subsequence
    // this reaches has, by construction, all of its adjacent gaps >= threshold.
    private static long CountAtLeast(int[] sorted, int k, int threshold) =>
        Memoizer.Memoize<(int Last, int Remaining), long>(
            (-1, k),
            (state, recurse) =>
            {
                if (state.Remaining == 0)
                {
                    return 1;
                }

                var total = 0L;

                for (var next = state.Last + 1; next < sorted.Length; next++)
                {
                    if (state.Last == -1 || sorted[next] - sorted[state.Last] >= threshold)
                    {
                        total += recurse((next, state.Remaining - 1));
                    }
                }

                return total;
            });
}
