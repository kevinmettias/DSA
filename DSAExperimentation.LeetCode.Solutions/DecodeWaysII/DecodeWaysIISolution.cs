using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Mathematics.Numerals;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.DecodeWaysII;

// LeetCode 639. Decode Ways II: DecodeWays' own single/pair recurrence, widened so
// '*' stands for "any digit 1-9" at a single position and the matching range of
// valid two-digit combinations at a pair, with every running total reduced mod
// 1e9+7 as LeetCode requires.
//
// Both strategies solve the same recurrence - decode(i) = the number of ways to
// decode digits[i..] - and differ only in evaluation order, exactly like DecodeWays:
// bottom-up tabulation fills an array from the end backwards, top-down
// memoization lets this repo's own Memoizer cache the same recurrence written as
// ordinary recursion.
[LeetCodeQuestion("decode-ways-ii", LeetCodeAnswerOrder.AsWritten)]
internal static class DecodeWaysIISolution
{
    // The textbook answer: a BCL long[] filled from the end, dp[i] = ways to
    // decode digits[i..]. Deliberately written without this repo's primitives - it is
    // the arm the memoized strategy below has to justify itself against.
    public static long CountDecodingsByTabulation(string digits)
    {
        var dp = new long[digits.Length + 1];
        dp[digits.Length] = 1;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            if (digits[i] == '0')
            {
                continue;
            }

            dp[i] = SingleWays(digits[i]) * dp[i + 1] % LeetCodeModulus.Value;

            if (i + 1 < digits.Length)
            {
                dp[i] = (dp[i] + (PairWays(digits[i], digits[i + 1]) * dp[i + DecodeWaysIIRecurrence.PairLength])) % LeetCodeModulus.Value;
            }
        }

        return dp[0];
    }

    private static long SingleWays(char symbol) => symbol == '*' ? DecodeWaysIIRecurrence.SingleWildcardWays : 1;

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

        var value = ((first - '0') * DecimalDigits.Size) + (second - '0');
        return value is >= DecimalDigits.Size and <= EnglishLetters.Size ? 1 : 0;
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
    public static long CountDecodingsByMemoization(string digits) =>
        Memoizer.Memoize<int, long>(0, new WildcardWaysFromDecodedIndex(digits));

    // The recurrence, as a named type: an index past the end is one way, a leading
    // '0' is none, and otherwise every way of writing the single character here is
    // carried forward from the next index, with every way of pairing it with the
    // following one added from the index after that.
    private sealed class WildcardWaysFromDecodedIndex(string digits) : IRecurrence<int, long>
    {
        public long Replay(int index, IRecurrence<int, long> rest)
        {
            if (index == digits.Length)
            {
                return 1;
            }

            if (digits[index] == '0')
            {
                return 0;
            }

            var total = SingleWays(digits[index]) * rest.Replay(index + 1, rest) % LeetCodeModulus.Value;

            if (index + 1 < digits.Length)
            {
                var paired = PairWays(digits[index], digits[index + 1]) * rest.Replay(index + DecodeWaysIIRecurrence.PairLength, rest);
                total = (total + paired) % LeetCodeModulus.Value;
            }

            return total;
        }
    }
}
