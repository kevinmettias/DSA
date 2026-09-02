using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DecodeWays;

// LeetCode 91. Decode Ways: count the ways a digit string decodes to a letter
// sequence under 'A'=1 .. 'Z'=26, where a '0' can only ever appear as the second
// digit of a valid two-digit group.
//
// Both strategies solve the same one-dimensional recurrence - decode(i) = the
// number of ways to decode s[i..] - and differ only in evaluation order: bottom-up
// tabulation fills an array from the end backwards, top-down memoization lets this
// repo's own Memoizer cache the same recurrence written as ordinary recursion.
internal static class DecodeWaysSolution
{
    private const int TwoDigitGroupLength = 2;
    private const int MaxTwoDigitCode = 26;

    // The textbook answer: a BCL int[] filled from the end, dp[i] = ways to decode
    // s[i..]. Deliberately written without this repo's primitives - it is the arm
    // the memoized strategy below has to justify itself against.
    public static int NumDecodingsByTabulation(string s)
    {
        var dp = new int[s.Length + 1];
        dp[s.Length] = 1;

        for (var i = s.Length - 1; i >= 0; i--)
        {
            FillTabulationCell(s, dp, i);
        }

        return dp[0];
    }

    private static void FillTabulationCell(string s, int[] dp, int i)
    {
        if (s[i] == '0')
        {
            return;
        }

        dp[i] = dp[i + 1];

        if (i + 1 >= s.Length)
        {
            return;
        }

        var twoDigit = int.Parse(s.AsSpan(i, TwoDigitGroupLength));

        if (twoDigit <= MaxTwoDigitCode)
        {
            dp[i] += dp[i + TwoDigitGroupLength];
        }
    }

    // This repo's own top-down engine: Memoizer.Memoize caches decode(i) the
    // first time each index is reached, so the recurrence reads as ordinary
    // recursion with no hand-rolled cache dictionary.
    public static int NumDecodingsByMemoization(string s)
    {
        return Memoizer.Memoize<int, int>(0, DecodeFrom);

        int DecodeFrom(int index, Func<int, int> decode)
        {
            if (index == s.Length)
            {
                return 1;
            }

            if (s[index] == '0')
            {
                return 0;
            }

            var total = decode(index + 1);

            if (index + 1 < s.Length)
            {
                var twoDigit = int.Parse(s.AsSpan(index, TwoDigitGroupLength));

                if (twoDigit <= MaxTwoDigitCode)
                {
                    total += decode(index + 2);
                }
            }

            return total;
        }
    }
}
