using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CountNoZeroPairsThatSumToN;

// LeetCode 3704. Count No-Zero Pairs That Sum to N: count ordered pairs (a, b) of
// no-zero-digit positive integers with a + b = targetSum. The naive baseline walks
// every split a in [1, targetSum), checks a and its partner `targetSum - a` digit by
// digit, and is correct but O(n) - hopeless once targetSum approaches its 10^15
// bound. The composed strategy is a digit DP over targetSum's decimal digits
// (least-significant first, with one appended guard digit to absorb a final carry):
// state (position, carry, aliveA, aliveB) tracks whether each number still has more
// digits left to place, so a shorter no-zero number is just one that "terminates"
// early and is padded with forced zero digits above that position - the same
// "arbitrary bespoke recurrence, memoized on a value-tuple state" shape Memoizer
// already serves for ClimbingStairsII and MinCostClimbingStairs.
internal static class CountNoZeroPairsThatSumToNSolution
{
    // A digit paired with whether the number it belongs to keeps going afterwards: a
    // live number may place any no-zero digit and then either continue or end right
    // there, while a terminated one may only place its padding zero and stays
    // terminated. Digit-major, which is the order the nested loops these replace
    // visited, so the memoized recurrence sees the same states in the same order.
    private static readonly (int Digit, bool NextAlive)[] NoZeroPlacements =
    [
        (1, true), (1, false), (2, true), (2, false), (3, true), (3, false),
        (4, true), (4, false), (5, true), (5, false), (6, true), (6, false),
        (7, true), (7, false), (8, true), (8, false), (9, true), (9, false),
    ];

    private static readonly (int Digit, bool NextAlive)[] PaddingPlacements = [(0, false)];

    // The textbook answer: check every split by trial division on digits, no repo
    // primitive - deliberately written this way, the arm the composed strategy
    // below has to justify itself against.
    public static long CountPairsByBruteForce(long targetSum)
    {
        long count = 0;

        for (var a = 1L; a < targetSum; a++)
        {
            var b = targetSum - a;

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
    public static long CountPairsByMemoizedDigitDp(long targetSum)
    {
        var digits = DigitsWithCarryGuard(targetSum);

        return Memoizer.Memoize<(int Position, int Carry, bool AliveA, bool AliveB), long>(
            (0, 0, true, true), new CountFromState(digits));
    }

    private static int[] DigitsWithCarryGuard(long number)
    {
        var digits = new List<int>();

        while (number > 0)
        {
            digits.Add((int)(number % 10));
            number /= 10;
        }

        digits.Add(0); // absorbs a final carry past number's own most significant digit
        return [.. digits];
    }

    // The digit rule, named: each column pairs every placement the two numbers can
    // still make and keeps the sums matching that column, so the state is the
    // position, the carry into it, and whether each number is still going.
    private sealed class CountFromState(int[] digits)
        : IRecurrence<(int Position, int Carry, bool AliveA, bool AliveB), long>
    {
        public long Replay(
            (int Position, int Carry, bool AliveA, bool AliveB) state,
            IRecurrence<(int Position, int Carry, bool AliveA, bool AliveB), long> rest)
        {
            if (state.Position == digits.Length)
            {
                return TerminalCount(state);
            }

            return PlacementCount(state, rest);
        }

        // Past the last column there is nothing left to place, so a state is either a
        // complete pair or it is not.
        private static long TerminalCount(
            (int Position, int Carry, bool AliveA, bool AliveB) state)
        {
            if (IsCompletePairWithoutCarry(state))
            {
                return 1L;
            }

            return 0L;
        }

        // Every digit pair the two numbers can still place, kept when the column's sum
        // matches the digit already sitting at this position.
        private long PlacementCount(
            (int Position, int Carry, bool AliveA, bool AliveB) state,
            IRecurrence<(int Position, int Carry, bool AliveA, bool AliveB), long> rest)
        {
            long total = 0;

            foreach (var (digitA, nextAliveA) in state.AliveA ? NoZeroPlacements : PaddingPlacements)
            {
                foreach (var (digitB, nextAliveB) in state.AliveB ? NoZeroPlacements : PaddingPlacements)
                {
                    var sum = digitA + digitB + state.Carry;

                    if (sum % 10 == digits[state.Position])
                    {
                        var next = (state.Position + 1, sum >= 10 ? 1 : 0, nextAliveA, nextAliveB);
                        total += rest.Replay(next, rest);
                    }
                }
            }

            return total;
        }
    }

    // The split is a genuine no-zero pair only once both numbers have run out of digits
    // and the last column left no carry to absorb.
    private static bool IsCompletePairWithoutCarry(
        (int Position, int Carry, bool AliveA, bool AliveB) state) =>
        state.Carry == 0 && !state.AliveA && !state.AliveB;
}
