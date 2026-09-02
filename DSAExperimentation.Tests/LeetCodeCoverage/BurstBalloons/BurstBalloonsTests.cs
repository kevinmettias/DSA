using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BurstBalloons;

// LeetCode 312. Burst Balloons: interval DP over (left, right) boundary pairs - the
// last balloon burst in a sub-range, not the first, is what makes the choice of k
// independent of every other choice in that range, since left and right survive as
// k's neighbors regardless of the order everything else inside them bursts. This
// repo's own Memoizer<TState,TResult> supplies the cache, keyed by that pair, the
// same 2-tuple-state shape EditDistanceBenchmarks.cs already uses.
public sealed partial class BurstBalloonsTests
{
    [Theory]
    [InlineData(new[] { 3, 1, 5, 8 }, 167)]
    [InlineData(new[] { 1, 5 }, 10)]
    [InlineData(new[] { 7 }, 7)]
    public void MaxCoins_LeetCodeExamples_ReturnsMaximumCoinsFromBursting(int[] nums, int expected)
        => Assert.Equal(expected, MaxCoins(nums));

    private static int MaxCoins(int[] nums)
    {
        var padded = BuildPaddedBoundary(nums);
        return Memoizer.Memoize<(int Left, int Right), int>(
            (0, padded.Length - 1), (range, coins) => CoinsBetween(range, coins, padded));
    }

    private static int[] BuildPaddedBoundary(int[] nums)
    {
        var padded = new int[nums.Length + 2];
        padded[0] = 1;
        padded[^1] = 1;
        Array.Copy(nums, 0, padded, 1, nums.Length);
        return padded;
    }

    private static int CoinsBetween((int Left, int Right) range, Func<(int Left, int Right), int> coins, int[] padded)
    {
        var (left, right) = range;
        if (right - left <= 1)
        {
            return 0;
        }

        var best = 0;
        for (var last = left + 1; last < right; last++)
        {
            var gained = (padded[left] * padded[last] * padded[right])
                + coins((left, last)) + coins((last, right));
            best = Math.Max(best, gained);
        }

        return best;
    }
}
