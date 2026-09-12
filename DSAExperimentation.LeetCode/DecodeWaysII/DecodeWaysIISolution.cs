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
    private const long Mod = 1_000_000_007;
    private const int PairLength = 2;
    private const int SingleWildcardWays = 9;
    private const int BothWildcardPairWays = 15;
    private const int StarThenSmallDigitWays = 2;
    private const int FirstIsOneStarWays = 9;
    private const int FirstIsTwoStarWays = 6;
    private const int DecimalBase = 10;
    private const int MaxLetterCode = 26;

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

            dp[i] = SingleWays(s[i]) * dp[i + 1] % Mod;

            if (i + 1 < s.Length)
            {
                dp[i] = (dp[i] + (PairWays(s[i], s[i + 1]) * dp[i + PairLength])) % Mod;
            }
        }

        return dp[0];
    }

    // This repo's own top-down engine: Memoizer.Memoize caches decode(i) the
    // first time each index is reached, so the recurrence reads as ordinary
    // recursion with no hand-rolled cache dictionary.
    public static long NumDecodingsByMemoization(string s)
    {
        return Memoizer.Memoize<int, long>(0, DecodeFrom);

        long DecodeFrom(int index, Func<int, long> decode)
        {
            if (index == s.Length)
            {
                return 1;
            }

            if (s[index] == '0')
            {
                return 0;
            }

            var total = SingleWays(s[index]) * decode(index + 1) % Mod;

            if (index + 1 < s.Length)
            {
                total = (total + (PairWays(s[index], s[index + 1]) * decode(index + PairLength))) % Mod;
            }

            return total;
        }
    }

    private static long SingleWays(char c) => c == '*' ? SingleWildcardWays : 1;

    private static long PairWays(char first, char second)
    {
        if (first == '*' && second == '*')
        {
            return BothWildcardPairWays;
        }

        if (first == '*')
        {
            return second <= '6' ? StarThenSmallDigitWays : 1;
        }

        if (second == '*')
        {
            return first == '1' ? FirstIsOneStarWays : first == '2' ? FirstIsTwoStarWays : 0;
        }

        var value = ((first - '0') * DecimalBase) + (second - '0');
        return value is >= DecimalBase and <= MaxLetterCode ? 1 : 0;
    }
}
