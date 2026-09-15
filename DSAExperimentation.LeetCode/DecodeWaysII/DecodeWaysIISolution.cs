using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DecodeWaysII;

// LeetCode 639. Decode Ways II: DecodeWays' own single/pair recurrence, widened so
// '*' stands for "any digit 1-9" at a single position and the matching range of
// valid two-digit combinations at a pair, with every running total reduced mod
// 1e9+7 as LeetCode requires.
//
// Both strategies solve the same recurrence - decode(i) = the number of ways to
// decode s[i..] - and differ only in evaluation order, exactly like DecodeWays:
// bottom-up tabulation fills an array from the end backwards, top-down
// memoization lets this repo's own Memoizer cache the same recurrence written as
// ordinary recursion.
internal static class DecodeWaysIISolution
{
    // The textbook answer: a BCL long[] filled from the end, dp[i] = ways to
    // decode s[i..]. Deliberately written without this repo's primitives - it is
    // the arm the memoized strategy below has to justify itself against.
    public static long NumDecodingsByTabulation(string s)
    {
        var dp = new long[s.Length + 1];
        dp[s.Length] = 1;

        for (var i = s.Length - 1; i >= 0; i--)
        {
            if (s[i] == '0')
            {
                continue;
            }

            dp[i] = SingleWays(s[i]) * dp[i + 1] % DecodeWaysIIRecurrence.Mod;

            if (i + 1 < s.Length)
            {
                dp[i] = (dp[i] + (PairWays(s[i], s[i + 1]) * dp[i + DecodeWaysIIRecurrence.PairLength])) % DecodeWaysIIRecurrence.Mod;
            }
        }

        return dp[0];
    }

    private static long SingleWays(char c) => c == '*' ? DecodeWaysIIRecurrence.SingleWildcardWays : 1;

    private static long PairWays(char first, char second)
    {
        if (first == '*' && second == '*')
        {
            return DecodeWaysIIRecurrence.BothWildcardPairWays;
        }

        if (first == '*')
        {
            return second <= '6' ? DecodeWaysIIRecurrence.StarThenSmallDigitWays : 1;
        }

        if (second == '*')
        {
            return SecondIsStarWays(first);
        }

        var value = ((first - '0') * DecodeWaysIIRecurrence.DecimalBase) + (second - '0');
        return value is >= DecodeWaysIIRecurrence.DecimalBase and <= DecodeWaysIIRecurrence.MaxLetterCode ? 1 : 0;
    }

    // '*' in the second position completes a two-digit code only behind a leading
    // '1' (11-19) or '2' (21-26); any other first digit leaves no code at all.
    private static long SecondIsStarWays(char first) => first switch
    {
        '1' => DecodeWaysIIRecurrence.FirstIsOneStarWays,
        '2' => DecodeWaysIIRecurrence.FirstIsTwoStarWays,
        _ => 0,
    };

    // This repo's own top-down engine: Memoizer.Memoize caches decode(i) the
    // first time each index is reached, so the recurrence reads as ordinary
    // recursion with no hand-rolled cache dictionary.
    public static long NumDecodingsByMemoization(string s) =>
        Memoizer.Memoize<int, long>(0, new WildcardWaysFromDecodedIndex(s));

    // The recurrence, as a named type: an index past the end is one way, a leading
    // '0' is none, and otherwise every way of writing the single character here is
    // carried forward from the next index, with every way of pairing it with the
    // following one added from the index after that.
    private sealed class WildcardWaysFromDecodedIndex(string s) : IRecurrence<int, long>
    {
        public long Replay(int index, IRecurrence<int, long> rest)
        {
            if (index == s.Length)
            {
                return 1;
            }

            if (s[index] == '0')
            {
                return 0;
            }

            var total = SingleWays(s[index]) * rest.Replay(index + 1, rest) % DecodeWaysIIRecurrence.Mod;

            if (index + 1 < s.Length)
            {
                var paired = PairWays(s[index], s[index + 1]) * rest.Replay(index + DecodeWaysIIRecurrence.PairLength, rest);
                total = (total + paired) % DecodeWaysIIRecurrence.Mod;
            }

            return total;
        }
    }
}
