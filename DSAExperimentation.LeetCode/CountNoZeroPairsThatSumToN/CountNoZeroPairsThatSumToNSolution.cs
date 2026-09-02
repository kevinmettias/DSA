using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CountNoZeroPairsThatSumToN;

// LeetCode 3704. Count No-Zero Pairs That Sum to N: count ordered pairs (a, b) of
// no-zero-digit positive integers with a + b = n. The naive baseline walks every
// split a in [1, n) and checks both a and n-a digit by digit - correct but O(n),
// hopeless once n approaches its 10^15 bound. The composed strategy is a digit DP
// over n's decimal digits (least-significant first, with one appended guard digit
// to absorb a final carry): state (position, carry, aliveA, aliveB) tracks whether
// each number still has more digits left to place, so a shorter no-zero number is
// just one that "terminates" early and is padded with forced zero digits above
// that position - the same "arbitrary bespoke recurrence, memoized on a value-tuple
// state" shape Memoizer already serves for ClimbingStairsII and MinCostClimbingStairs.
internal static class CountNoZeroPairsThatSumToNSolution
{
    private static readonly int[] NoZeroDigits = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    private static readonly int[] ForcedZeroDigit = [0];
    private static readonly bool[] AliveOrTerminated = [true, false];
    private static readonly bool[] TerminatedOnly = [false];

    // The textbook answer: check every split by trial division on digits, no repo
    // primitive - deliberately written this way, the arm the composed strategy
    // below has to justify itself against.
    public static long CountPairsByBruteForce(long n)
    {
        long count = 0;

        for (var a = 1L; a < n; a++)
        {
            var b = n - a;

            if (HasNoZeroDigit(a) && HasNoZeroDigit(b))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasNoZeroDigit(long value)
    {
        while (value > 0)
        {
            if (value % 10 == 0)
            {
                return false;
            }

            value /= 10;
        }

        return true;
    }

    // This repo's own Memoizer over the digit-DP recurrence described above.
    public static long CountPairsByMemoizedDigitDp(long n)
    {
        var digits = DigitsWithCarryGuard(n);

        return Memoizer.Memoize<(int Position, int Carry, bool AliveA, bool AliveB), long>(
            (0, 0, true, true), (state, countFrom) => CountFromState(state, digits, countFrom));
    }

    private static int[] DigitsWithCarryGuard(long n)
    {
        var digits = new List<int>();

        while (n > 0)
        {
            digits.Add((int)(n % 10));
            n /= 10;
        }

        digits.Add(0); // absorbs a final carry past n's own most significant digit
        return [.. digits];
    }

    private static long CountFromState(
        (int Position, int Carry, bool AliveA, bool AliveB) state,
        int[] digits,
        Func<(int Position, int Carry, bool AliveA, bool AliveB), long> countFrom)
    {
        var (position, carry, aliveA, aliveB) = state;

        if (position == digits.Length)
        {
            return carry == 0 && !aliveA && !aliveB ? 1L : 0L;
        }

        var required = digits[position];
        long total = 0;

        foreach (var digitA in aliveA ? NoZeroDigits : ForcedZeroDigit)
        {
            foreach (var nextAliveA in aliveA ? AliveOrTerminated : TerminatedOnly)
            {
                foreach (var digitB in aliveB ? NoZeroDigits : ForcedZeroDigit)
                {
                    foreach (var nextAliveB in aliveB ? AliveOrTerminated : TerminatedOnly)
                    {
                        var sum = digitA + digitB + carry;

                        if (sum % 10 != required)
                        {
                            continue;
                        }

                        total += countFrom((position + 1, sum >= 10 ? 1 : 0, nextAliveA, nextAliveB));
                    }
                }
            }
        }

        return total;
    }
}
