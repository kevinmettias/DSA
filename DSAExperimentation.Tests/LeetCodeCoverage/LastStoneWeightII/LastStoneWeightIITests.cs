using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastStoneWeightII;

// LeetCode 1049. Last Stone Weight II: the smashing process always reduces to
// picking a subset whose sum is as close as possible to half the total weight
// (one pile keeps that subset, the other keeps the rest) - the same 0/1-knapsack
// subset-sum recurrence PartitionEqualSubsetSumTests uses, generalized from
// "can we hit half exactly" to "what's the closest achievable sum to half," via
// this repo's own Memoizer instead of the textbook un-memoized exponential
// recursion.
public sealed partial class LastStoneWeightIITests
{
    [Theory]
    [InlineData(new[] { 2, 7, 4, 1, 8, 1 }, 1)]
    [InlineData(new[] { 31, 26, 33, 21, 40 }, 5)]
    [InlineData(new[] { 1 }, 1)]
    public void LastStoneWeightII_LeetCodeExamples_ReturnsMinimumPossibleWeight(int[] stones, int expected)
        => Assert.Equal(expected, LastStoneWeightII(stones));

    private static int LastStoneWeightII(int[] stones)
    {
        var total = stones.Sum();
        var half = total / 2;

        var closestToHalf = Memoizer.Memoize<(int Index, int Capacity), int>((0, half), BestReachableSum);
        return total - 2 * closestToHalf;

        int BestReachableSum((int Index, int Capacity) state, Func<(int Index, int Capacity), int> bestReachableSum)
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
}
