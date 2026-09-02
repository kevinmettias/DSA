using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPeopleAwareOfASecret;

// LeetCode 2327. Number of People Aware of a Secret: dp[day] = how many people
// FIRST learn the secret on that day. Day i's newcomers are the sum of dp[j] over
// every day j still actively sharing on day i (from delay days after learning up to
// forget-1 days after) - a contiguous range sum this repo's own
// FenwickTree<long,SumOperation<long>> answers via Add/Query in O(log n), instead of
// re-summing the whole sliding window from scratch on every day. The final answer
// sums dp[day] for every day whose people haven't forgotten by day n yet - the same
// range-query shape, just against the tail of the array.
public sealed class NumberOfPeopleAwareOfASecretTests
{
    private const long Mod = 1_000_000_007L;

    [Theory]
    [InlineData(6, 2, 4, 5)]
    [InlineData(4, 1, 3, 6)]
    [InlineData(2, 1, 2, 2)]
    public void PeopleWithSecret_LeetCodeExamples_ReturnsExpectedCount(int n, int delay, int forget, long expected)
        => Assert.Equal(expected, PeopleWithSecret(n, delay, forget));

    private static long PeopleWithSecret(int n, int delay, int forget)
    {
        var dp = new FenwickTree<long, SumOperation<long>>(n);
        dp.Add(0, 1);

        for (var day = 2; day <= n; day++)
        {
            var low = Math.Max(1, day - forget + 1);
            var high = day - delay;

            if (high >= low)
            {
                dp.Add(day - 1, dp.Query(low - 1, high - 1) % Mod);
            }
        }

        var finalLow = Math.Max(1, n - forget + 1);
        return dp.Query(finalLow - 1, n - 1) % Mod;
    }
}
