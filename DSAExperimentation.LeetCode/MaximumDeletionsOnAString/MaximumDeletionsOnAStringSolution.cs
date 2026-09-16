using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.MaximumDeletionsOnAString;

// LeetCode 2430. Maximum Deletions on a String: one operation either deletes the
// whole remaining string, or deletes a prefix s[i..i+k) when the equal-length block
// after it repeats that prefix exactly (s[i..i+k) == s[i+k..i+2k)). Maximize the
// number of operations.
//
// Both strategies run the same suffix DP: dp[i] = 1 + the best dp[i+k] over every
// half-length k in [1, (n-i)/2] whose prefix is immediately repeated, or 1 alone
// when no such k exists (the only remaining move is "delete the entire rest of s").
// Picking the SMALLEST valid k is not always optimal - a larger first deletion can
// leave a suffix with a richer split later, which "aaabaab" demonstrates by hand
// (deleting "aab" via k=3, not the also-valid k=1, is what reaches 4 operations
// instead of 3) - so every valid k's dp[i+k] is considered and the best one wins.
//
// The strategies differ only in what one candidate's half-equality test costs.
internal static class MaximumDeletionsOnAStringSolution
{
    // A deletable prefix is exactly two equal-length halves back to back.
    private const int RepeatedPrefixHalfCount = 2;

    // The textbook answer: cut both halves out as real substrings and compare them
    // with ==, paying an O(k) allocation and compare on every candidate whether it
    // matches or not. Deliberately written without this repo's primitives - it is
    // the arm the composed strategy below has to justify itself against.
    public static int MaxOperationsByNaiveSubstringComparison(string text)
    {
        var n = text.Length;
        var dp = new int[n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            var best = 0;

            for (var k = 1; i + (RepeatedPrefixHalfCount * k) <= n; k++)
            {
                var first = text.Substring(i, k);
                var second = text.Substring(i + k, k);

                if (first == second)
                {
                    best = Math.Max(best, dp[i + k]);
                }
            }

            dp[i] = 1 + best;
        }

        return dp[0];
    }

    // This repo's own RollingHash as an O(1) equality screen per DP transition, only
    // paying for a real SequenceEqual once the screen passes - the same
    // screen-then-verify shape LC 1316 and RollingHashSearch.FindAll use, since a
    // hash match is "probably equal", not "definitely equal", applied here per DP
    // transition instead of per dedup candidate. No substring is ever materialized,
    // so a candidate that fails costs two array lookups instead of two allocations.
    public static int MaxOperationsByRollingHashScreen(string text)
    {
        var n = text.Length;
        var hash = new RollingHash(text);
        var dp = new int[n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            dp[i] = 1 + BestContinuation(text, hash, dp, i);
        }

        return dp[0];
    }

    private static int BestContinuation(string text, RollingHash hash, int[] dp, int startIndex)
    {
        var n = text.Length;
        var best = 0;

        for (var k = 1; startIndex + (RepeatedPrefixHalfCount * k) <= n; k++)
        {
            if (IsRepeatedPrefix(text, hash, startIndex, k))
            {
                best = Math.Max(best, dp[startIndex + k]);
            }
        }

        return best;
    }

    private static bool IsRepeatedPrefix(string text, RollingHash hash, int start, int halfLength)
    {
        if (hash.Hash(start, halfLength) != hash.Hash(start + halfLength, halfLength))
        {
            return false;
        }

        var firstHalf = text.AsSpan(start, halfLength);
        var secondHalf = text.AsSpan(start + halfLength, halfLength);

        return firstHalf.SequenceEqual(secondHalf);
    }
}
