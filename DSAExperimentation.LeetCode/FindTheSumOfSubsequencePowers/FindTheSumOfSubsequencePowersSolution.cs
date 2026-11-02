using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Sorting;

namespace DSAExperimentation.LeetCode.FindTheSumOfSubsequencePowers;

// LeetCode 3098. Find the Sum of Subsequence Powers: sum, over every subsequence
// of nums with subsequenceLength elements, the minimum absolute difference between
// any two of its elements ("power"), mod 1e9+7.
//
// Sorted, a subsequence's power is just the minimum GAP between its consecutively
// chosen elements. The composed strategy leans on the identity
// power(S) = sum_{v=1}^{maxGap} [every adjacent gap in S is >= v]: summing that over
// every subsequenceLength-element subsequence swaps the order to
// sum_v (count of subsequenceLength-element subsequences whose adjacent gaps are all >= v), and that
// inner count is only a step function of v, changing at the handful of actual
// pairwise gaps present in nums - so it is enough to evaluate it once per distinct
// gap and weight by how wide a run of v it covers.
internal static class FindTheSumOfSubsequencePowersSolution
{
    // The textbook answer: enumerate every subsequenceLength-element subsequence
    // directly (2^n branch/skip choices), tracking the running minimum gap as it is
    // built. Deliberately plain recursion, no memoization and no repo primitives -
    // the arm the threshold-counting strategy below has to justify itself against.
    public static int SumOfPowersByBruteForce(int[] nums, int subsequenceLength)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);

        var total = SumPowers(
            sorted, subsequenceLength, index: 0, chain: (Picked: 0, LastValue: 0, MinGap: long.MaxValue));

        return (int)(total % LeetCodeModulus.Value);
    }

    // Reuses this repo's own Sort (MergeSort over an ArrayIndexedSequence) for both
    // sorts it needs, and Algorithms.DynamicProgramming.Memoizer for the per-threshold
    // "how many subsequenceLength-element subsequences survive this minimum gap"
    // count, and LeetCodeModulus for LeetCode's own 1e9+7 reporting convention.
    public static int SumOfPowersByThresholdCounting(int[] nums, int subsequenceLength)
    {
        var sorted = MergeSort.SortedCopy(nums);
        var gaps = PairwiseGaps(sorted);
        var thresholds = MergeSort.SortedCopy(gaps);

        return (int)SumThresholdContributions(sorted, subsequenceLength, thresholds);
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

    // Every threshold in a run of equal widths contributes the same number of
    // subsequenceLength-element subsequences, so the count is asked once per run and
    // weighted by how many thresholds that run covers.
    private static long SumThresholdContributions(int[] sorted, int subsequenceLength, int[] thresholds)
    {
        var total = 0L;
        var previous = 0;

        foreach (var threshold in thresholds)
        {
            var width = threshold - previous;

            if (width > 0)
            {
                var count = CountAtLeast(sorted, subsequenceLength, threshold) % LeetCodeModulus.Value;
                total = (total + width * count) % LeetCodeModulus.Value;
            }

            previous = threshold;
        }

        return total;
    }

    // State (Last, Remaining): Last = -1 means "nothing chosen yet" (any element may
    // start the chain); Remaining counts elements still needed. Every subsequence
    // this reaches has, by construction, all of its adjacent gaps >= threshold.
    private static long CountAtLeast(int[] sorted, int subsequenceLength, int threshold) =>
        Memoizer.Memoize<(int Last, int Remaining), long>(
            (-1, subsequenceLength), new ChainsWiderThan(sorted, threshold));

    /// <summary>
    /// The recurrence, named: state (Last, Remaining) counts the chains of Remaining
    /// more elements whose every adjacent gap clears <paramref name="threshold"/> -
    /// Last = -1 meaning nothing is chosen yet, so any element may start the chain.
    /// </summary>
    private sealed class ChainsWiderThan(int[] sorted, int threshold) : IRecurrence<(int Last, int Remaining), long>
    {
        /// <inheritdoc/>
        public long Replay((int Last, int Remaining) state, IRecurrence<(int Last, int Remaining), long> rest)
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
                    total += rest.Replay((next, state.Remaining - 1), rest);
                }
            }

            return total;
        }
    }

    // The chain built so far is one thing - how many elements it holds, its last value,
    // and the smallest gap among its adjacent pairs - which is why `picked == 0` below
    // can stand for "nothing chosen yet"; `index` is just the cursor over `sorted`.
    private static long SumPowers(
        int[] sorted, int subsequenceLength, int index, (int Picked, int LastValue, long MinGap) chain)
    {
        var (picked, lastValue, minGap) = chain;

        if (picked == subsequenceLength)
        {
            return minGap;
        }

        if (index == sorted.Length)
        {
            return 0;
        }

        var nextMinGap = picked == 0 ? long.MaxValue : Math.Min(minGap, sorted[index] - lastValue);
        var take = SumPowers(sorted, subsequenceLength, index + 1, (picked + 1, sorted[index], nextMinGap));
        var skip = SumPowers(sorted, subsequenceLength, index + 1, chain);

        return take + skip;
    }
}
