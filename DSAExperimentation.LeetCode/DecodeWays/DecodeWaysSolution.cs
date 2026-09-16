using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DecodeWays;

// LeetCode 91. Decode Ways: count the ways a digit string decodes to a letter
// sequence under 'A'=1 .. 'Z'=26, where a '0' can only ever appear as the second
// digit of a valid two-digit group.
//
// Both strategies solve the same one-dimensional recurrence - decode(i) = the
// number of ways to decode digits[i..] - and differ only in evaluation order: bottom-up
// tabulation fills an array from the end backwards, top-down memoization lets this
// repo's own Memoizer cache the same recurrence written as ordinary recursion.
internal static class DecodeWaysSolution
{
    private const int TwoDigitGroupLength = 2;
    private const int MaxTwoDigitCode = 26;

    // The textbook answer: a BCL int[] filled from the end, dp[index] = ways to decode
    // digits[index..]. Deliberately written without this repo's primitives - it is the arm
    // the memoized strategy below has to justify itself against.
    public static int CountDecodingsByTabulation(string digits)
    {
        var dp = new int[digits.Length + 1];
        dp[digits.Length] = 1;

        for (var index = digits.Length - 1; index >= 0; index--)
        {
            FillTabulationCell(digits, dp, index);
        }

        return dp[0];
    }

    private static void FillTabulationCell(string digits, int[] dp, int index)
    {
        if (digits[index] == '0')
        {
            return;
        }

        dp[index] = dp[index + 1];

        if (index + 1 >= digits.Length)
        {
            return;
        }

        var span = digits.AsSpan(index, TwoDigitGroupLength);
        var twoDigit = int.Parse(span);

        if (twoDigit <= MaxTwoDigitCode)
        {
            dp[index] += dp[index + TwoDigitGroupLength];
        }
    }

    // This repo's own top-down engine: Memoizer.Memoize caches decode(i) the
    // first time each index is reached, so the recurrence reads as ordinary
    // recursion with no hand-rolled cache dictionary.
    public static int CountDecodingsByMemoization(string digits) =>
        Memoizer.Memoize<int, int>(0, new WaysFromDecodedIndex(digits));

    // Whether the pair starting at `index` spells a code at all: false when there
    // is no room for a second digit, and false when the pair runs past 'Z'.
    private static bool HasTwoDigitGroup(string digits, int index)
    {
        if (index + 1 >= digits.Length)
        {
            return false;
        }

        var span = digits.AsSpan(index, TwoDigitGroupLength);
        var twoDigit = int.Parse(span);

        return twoDigit <= MaxTwoDigitCode;
    }

    // The recurrence, as a named type: an index past the end is one way, a leading
    // '0' is none, and otherwise one digit carries the ways from the next index
    // while a pair that spells a code adds the ways from two on.
    private sealed class WaysFromDecodedIndex(string digits) : IRecurrence<int, int>
    {
        public int Replay(int index, IRecurrence<int, int> rest)
        {
            if (index == digits.Length)
            {
                return 1;
            }

            if (digits[index] == '0')
            {
                return 0;
            }

            var total = rest.Replay(index + 1, rest);

            if (HasTwoDigitGroup(digits, index))
            {
                total += rest.Replay(index + TwoDigitGroupLength, rest);
            }

            return total;
        }
    }
}
