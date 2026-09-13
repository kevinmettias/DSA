using DSAExperimentation.Algorithms.DynamicProgramming;

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
    private const int HalfDivisor = 2;
    private const int PartitionDifferenceMultiplier = 2;

    // Baseline: the textbook bottom-up tabulation over a single rolling int[]
    // capacity row, walked high-to-low so each stone is used at most once.
    public static int MinWeightByTabulation(int[] stones)
    {
        var total = stones.Sum();
        var half = total / HalfDivisor;
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
        var half = total / HalfDivisor;

        var closestToHalf = Memoizer.Memoize<(int Index, int Capacity), int>(
            (0, half), (state, bestReachableSum) => BestReachableSum(state, bestReachableSum, stones));

        return total - PartitionDifferenceMultiplier * closestToHalf;
    }

    private static int BestReachableSum(
        (int Index, int Capacity) state, Func<(int Index, int Capacity), int> bestReachableSum, int[] stones)
    {
        if (state.Index == stones.Length)
        {
            return 0;
        }

        var skip = bestReachableSum((state.Index + 1, state.Capacity));

        if (stones[state.Index] > state.Capacity)
        {
            return skip;
        }

        var take = stones[state.Index] + bestReachableSum((state.Index + 1, state.Capacity - stones[state.Index]));
        return Math.Max(skip, take);
    }
}
