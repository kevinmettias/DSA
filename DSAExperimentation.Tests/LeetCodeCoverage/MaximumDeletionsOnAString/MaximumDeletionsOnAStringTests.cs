using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumDeletionsOnAString;

// LeetCode 2430. Maximum Deletions on a String: dp[i] = 1 + the best dp[i+k] over
// every half-length k in [1, (n-i)/2] where s[i..i+k) repeats immediately after
// itself (s[i..i+k) == s[i+k..i+2k)), or 1 alone when no such k exists (the only
// remaining move is "delete the entire rest of s" in one operation). Picking the
// SMALLEST valid k is not always optimal - a larger first deletion can still leave a
// suffix with a richer split later, verified by hand below against "aaabaab"
// (deleting "aab" via k=3 first, not the also-valid k=1, is what reaches 4
// operations instead of 3) - so every valid k's dp[i+k] is considered and the best
// one wins. Equality is checked with the same screen-then-verify shape
// DistinctEchoSubstringsTests already uses via this repo's own RollingHash (an O(1)
// hash compare before paying for a real SequenceEqual), applied here per DP
// transition instead of per dedup candidate.
public sealed partial class MaximumDeletionsOnAStringTests
{
    [Theory]
    [InlineData("aaaaa", 5)]
    [InlineData("aaabaab", 4)]
    [InlineData("abcabcabc", 3)]
    [InlineData("a", 1)]
    public void MaxOperations_HandVerifiedExamples_ReturnsExpectedOperationCount(string s, int expected)
        => Assert.Equal(expected, MaxOperations(s));

    private static int MaxOperations(string s)
    {
        var n = s.Length;
        var hash = new RollingHash(s);
        var dp = new int[n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            dp[i] = 1 + BestContinuation(s, hash, dp, i);
        }

        return dp[0];
    }

    private static int BestContinuation(string s, RollingHash hash, int[] dp, int i)
    {
        var n = s.Length;
        var best = 0;

        for (var k = 1; i + (2 * k) <= n; k++)
        {
            if (IsDuplicatedPrefix(s, hash, i, k))
            {
                best = Math.Max(best, dp[i + k]);
            }
        }

        return best;
    }

    private static bool IsDuplicatedPrefix(string s, RollingHash hash, int start, int halfLength)
    {
        if (hash.Hash(start, halfLength) != hash.Hash(start + halfLength, halfLength))
        {
            return false;
        }

        var firstHalf = s.AsSpan(start, halfLength);
        var secondHalf = s.AsSpan(start + halfLength, halfLength);
        return firstHalf.SequenceEqual(secondHalf);
    }
}
