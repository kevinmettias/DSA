using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.NumberOfBeautifulIntegersInTheRange;

// LeetCode 2827. Number of Beautiful Integers in the Range: count integers x in
// [low, high] where x's decimal digits contain an equal count of even and odd
// digits AND x % k == 0. CountByBruteForce walks the range directly and checks
// both conditions per integer - the O(range * digits) arm the digit-DP has to
// beat. CountByDigitDpMemo answers CountUpTo(high) - CountUpTo(low - 1) (the
// standard "count up to N" trick), where CountUpTo(n) walks n's own digit string
// with Memoizer over (Position, Tight, Started, Diff, Remainder):
//   - Tight: whether every digit placed so far equals n's own digit at that
//     position (still bound by n) or has already gone strictly below it (free
//     to place any digit 0-9 from here on) - the classic digit-DP bound.
//   - Started: whether a non-zero digit has been placed yet, so leading
//     zero-padding (numbers shorter than n's own digit count) doesn't get
//     miscounted as an actual "0" digit toward Diff.
//   - Diff: (even digit count) - (odd digit count) so far, only accumulated
//     once Started is true; a beautiful number needs this at exactly 0.
//   - Remainder: the running value mod k, so divisibility falls out of the
//     same walk instead of a second pass.
// Same "tuple state, Memoizer.Memoize<TState,TResult>" composition
// CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo already proves
// out for a different counting recurrence.
internal static class NumberOfBeautifulIntegersInTheRangeSolution
{
    public static long CountByBruteForce(int low, int high, int k)
    {
        var count = 0L;

        for (var x = low; x <= high; x++)
        {
            if (x % k == 0 && HasEqualEvenOddDigits(x))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasEqualEvenOddDigits(int x)
    {
        var diff = 0;

        foreach (var c in x.ToString())
        {
            diff += (c - '0') % 2 == 0 ? 1 : -1;
        }

        return diff == 0;
    }

    public static long CountByDigitDpMemo(int low, int high, int k)
        => CountUpTo(high, k) - CountUpTo(low - 1, k);

    private static long CountUpTo(long n, int k)
    {
        if (n < 0)
        {
            return 0;
        }

        var digits = n.ToString();

        return Memoizer.Memoize<(int Position, bool Tight, bool Started, int Diff, int Remainder), long>(
            (0, true, false, 0, 0),
            (state, recurse) => CountFrom(state, digits, k, recurse));
    }

    private static long CountFrom(
        (int Position, bool Tight, bool Started, int Diff, int Remainder) state,
        string digits,
        int k,
        Func<(int Position, bool Tight, bool Started, int Diff, int Remainder), long> recurse)
    {
        if (state.Position == digits.Length)
        {
            return state.Started && state.Diff == 0 && state.Remainder == 0 ? 1 : 0;
        }

        var limit = state.Tight ? digits[state.Position] - '0' : 9;
        var total = 0L;

        for (var digit = 0; digit <= limit; digit++)
        {
            var started = state.Started || digit != 0;
            var diff = started ? state.Diff + (digit % 2 == 0 ? 1 : -1) : state.Diff;
            var remainder = (state.Remainder * 10 + digit) % k;

            total += recurse((state.Position + 1, state.Tight && digit == limit, started, diff, remainder));
        }

        return total;
    }
}
