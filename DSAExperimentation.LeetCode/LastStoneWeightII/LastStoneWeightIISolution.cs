using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.LastStoneWeightII;

// LeetCode 1049. Last Stone Weight II: smashing stones pairwise always reduces to
// splitting them into two piles and reporting the difference of their sums, so the
// answer is total - 2 * (the largest subset sum that does not exceed half the total)
// - the 0/1-knapsack subset-sum recurrence PartitionEqualSubsetSum uses, generalized
// from "can we hit half exactly" to "how close to half can we get".
//
// Both strategies walk that same recurrence in O(stones.Length * half); they differ
// only in direction - a bottom-up capacity table versus this repo's Memoizer driving
// the recursion top-down.
internal static class LastStoneWeightIISolution
{
    private const int PartitionDifferenceMultiplier = 2;

    // Baseline: the textbook bottom-up tabulation over a single rolling int[]
    // capacity row, walked high-to-low so each stone is used at most once.
    public static int MinWeightByTabulation(int[] stones)
    {
        var total = stones.Sum();
        var half = total / AlgorithmConstants.HalvingFactor;
        var bestSumForCapacity = new int[half + 1];

        foreach (var stone in stones)
        {
            for (var capacity = half; capacity >= stone; capacity--)
            {
                bestSumForCapacity[capacity] =
                    Math.Max(bestSumForCapacity[capacity], bestSumForCapacity[capacity - stone] + stone);
            }
        }

        return total - PartitionDifferenceMultiplier * bestSumForCapacity[half];
    }

    // Same recurrence top-down: (index, remaining capacity) -> best reachable sum,
    // with this repo's Memoizer owning the cache so the exponential take/skip tree
    // collapses to one evaluation per state.
    public static int MinWeightByMemoizer(int[] stones)
    {
        var total = stones.Sum();
        var half = total / AlgorithmConstants.HalvingFactor;

        var closestToHalf = Memoizer.Memoize<(int Index, int Capacity), int>(
            (0, half), new BestReachableSum(stones));

        return total - PartitionDifferenceMultiplier * closestToHalf;
    }

    // The take/skip rule, named: an exhausted stone list reaches no further sum, and
    // otherwise each stone is either skipped or taken once it still fits the capacity.
    private sealed class BestReachableSum(int[] stones) : IRecurrence<(int Index, int Capacity), int>
    {
        /// <inheritdoc/>
        public int Replay((int Index, int Capacity) state, IRecurrence<(int Index, int Capacity), int> rest)
        {
            if (state.Index == stones.Length)
            {
                return 0;
            }

            var skip = rest.Replay((state.Index + 1, state.Capacity), rest);

            if (stones[state.Index] > state.Capacity)
            {
                return skip;
            }

            var take = stones[state.Index]
                + rest.Replay((state.Index + 1, state.Capacity - stones[state.Index]), rest);

            return Math.Max(skip, take);
        }
    }
}
