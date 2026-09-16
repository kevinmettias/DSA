using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.NumberOfBeautifulIntegersInTheRange;

// LeetCode 2827. Number of Beautiful Integers in the Range: count integers in
// [low, high] whose decimal digits contain an equal count of even and odd digits
// AND which are divisible by divisor. CountByBruteForce walks the range directly
// and checks both conditions per integer - the O(range * digits) arm the digit-DP
// has to beat. CountByDigitDpMemo answers CountUpTo(high) - CountUpTo(low - 1)
// (the standard "count up to N" trick), where CountUpTo(upperBound) walks
// upperBound's own digit string with Memoizer over
// (Position, Tight, Started, Diff, Remainder):
//   - Tight: whether every digit placed so far equals upperBound's own digit at
//     that position (still bound by upperBound) or has already gone strictly
//     below it (free to place any digit 0-9 from here on) - the classic
//     digit-DP bound.
//   - Started: whether a non-zero digit has been placed yet, so leading
//     zero-padding (numbers shorter than upperBound's own digit count) doesn't
//     get miscounted as an actual "0" digit toward Diff.
//   - Diff: (even digit count) - (odd digit count) so far, only accumulated
//     once Started is true; a beautiful number needs this at exactly 0.
//   - Remainder: the running value mod divisor, so divisibility falls out of the
//     same walk instead of a second pass.
// Same "tuple state, Memoizer.Memoize<TState,TResult>" composition
// CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo already proves
// out for a different counting recurrence.
internal static class NumberOfBeautifulIntegersInTheRangeSolution
{
    public static long CountByBruteForce(int low, int high, int divisor)
    {
        var count = 0L;

        for (var x = low; x <= high; x++)
        {
            if (x % divisor == 0 && HasEqualEvenOddDigits(x))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasEqualEvenOddDigits(int value)
    {
        var diff = 0;

        foreach (var c in value.ToString())
        {
            diff += DiffContribution(c - '0');
        }

        return diff == 0;
    }

    public static long CountByDigitDpMemo(int low, int high, int divisor)
        => CountUpTo(high, divisor) - CountUpTo(low - 1, divisor);

    private static long CountUpTo(long upperBound, int divisor)
    {
        if (upperBound < 0)
        {
            return 0;
        }

        var digits = upperBound.ToString();

        return Memoizer.Memoize<(int Position, bool Tight, bool Started, int Diff, int Remainder), long>(
            (0, true, false, 0, 0),
            new BeautifulCountsFrom(digits, divisor));
    }

    // Every digit has been placed by the time this is asked: the number is beautiful when
    // it has started (leading zero-padding is not a number), its even and odd digit counts
    // cancel out, and it divides by divisor.
    private static bool IsBeautifulNumber(
        (int Position, bool Tight, bool Started, int Diff, int Remainder) state) =>
        state.Started && state.Diff == 0 && state.Remainder == 0;

    private static int DigitValue(string digits, int position) => digits[position] - '0';

    // The walk's Started flag and running even-minus-odd count once `digit` is placed: the
    // first non-zero digit starts the number, and every digit after that moves Diff by one,
    // while the leading zeros placed before it are padding and leave Diff standing.
    private static (bool Started, int Diff) DiffAfterDigit(
        (int Position, bool Tight, bool Started, int Diff, int Remainder) state, int digit)
    {
        if (!state.Started && digit == 0)
        {
            return (false, state.Diff);
        }

        return (true, state.Diff + DiffContribution(digit));
    }

    // A digit's contribution to Diff, which counts even digits minus odd ones.
    private static int DiffContribution(int digit)
    {
        if (digit % 2 == 0)
        {
            return 1;
        }

        return -1;
    }

    // The digit-DP recurrence, as a named type: walk the number's own digits left to
    // right, place every digit the tight bound still allows, and count the completions
    // that land on a beautiful number.
    private sealed class BeautifulCountsFrom(string digits, int divisor)
        : IRecurrence<(int Position, bool Tight, bool Started, int Diff, int Remainder), long>
    {
        public long Replay(
            (int Position, bool Tight, bool Started, int Diff, int Remainder) state,
            IRecurrence<(int Position, bool Tight, bool Started, int Diff, int Remainder), long> rest)
        {
            if (state.Position == digits.Length)
            {
                return IsBeautifulNumber(state) ? 1 : 0;
            }

            var limit = state.Tight ? DigitValue(digits, state.Position) : 9;
            var total = 0L;

            for (var digit = 0; digit <= limit; digit++)
            {
                var (started, diff) = DiffAfterDigit(state, digit);
                var remainder = (state.Remainder * 10 + digit) % divisor;

                total += rest.Replay(
                    (state.Position + 1, state.Tight && digit == limit, started, diff, remainder),
                    rest);
            }

            return total;
        }
    }
}
