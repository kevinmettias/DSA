using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumNumberOfDaysToEatNOranges;

// LeetCode 1553. Minimum Number of Days to Eat N Oranges: each day you may eat one
// orange, or half of them when the count is even, or two thirds when it is divisible
// by three. The fewest days is days(n) = n when n <= 1, else
// 1 + min(n % 2 + days(n / 2), n % 3 + days(n / 3)) - eat off the remainder one at a
// time, then spend a single day halving or thirding whatever is left.
//
// Both strategies walk that exact recurrence; the only difference is whether
// repeated states are cached. The orange count stays int, not long: the problem's own
// stated upper bound (2*10^9) is below int.MaxValue, so unlike IntegerReplacement's
// n+1 branch there is no widening to do here.
internal static class MinimumNumberOfDaysToEatNOrangesSolution
{
    // The recurrence's two branches: eat n%2 one at a time then halve the rest, or
    // eat n%3 one at a time then take a third of the rest.
    private const int EatOneAtATimeThenHalveDivisor = 2;
    private const int EatOneAtATimeThenThirdDivisor = 3;

    // The textbook answer: plain unmemoized recursion. The two subproblems (n/2 and
    // n/3) reconverge heavily across levels once floors are applied, so this arm's
    // call tree revisits the same values over and over - the arm the memoized
    // recurrence below has to justify itself against.
    public static int MinDaysByUnmemoizedRecursion(int orangeCount) => Eat(orangeCount);

    // This repo's own Memoizer-driven DP recurrence (same IntegerReplacementSolution/
    // FibonacciNumberSolution composition) over the identical recurrence above -
    // caching collapses that reconverging call tree down to the handful of distinct
    // states a chain of halvings and thirdings can actually reach.
    public static int MinDaysByMemoizedRecurrence(int orangeCount)
        => Memoizer.Memoize(orangeCount, new DaysFromOrangeCount());

    // The recurrence, as a named type: with at most one orange left there is nothing
    // to choose, and otherwise the day's work is eating n%2 (or n%3) of them one at a
    // time and then halving (or thirding) the rest, whichever of the two is cheaper.
    private sealed class DaysFromOrangeCount : IRecurrence<int, int>
    {
        public int Replay(int orangeCount, IRecurrence<int, int> rest)
        {
            if (orangeCount <= 1)
            {
                return orangeCount;
            }

            var halvingDays = (orangeCount % EatOneAtATimeThenHalveDivisor)
                + rest.Replay(orangeCount / EatOneAtATimeThenHalveDivisor, rest);
            var thirdingDays = (orangeCount % EatOneAtATimeThenThirdDivisor)
                + rest.Replay(orangeCount / EatOneAtATimeThenThirdDivisor, rest);

            return 1 + Math.Min(halvingDays, thirdingDays);
        }
    }

    private static int Eat(int orangeCount)
        => orangeCount <= 1
            ? orangeCount
            : EatAfterHalvingOrThirding(orangeCount);

    // Eat the remainder one orange at a time, then spend a single day halving or
    // thirding what is left - taking whichever of the two branches costs fewer days
    // in total. The memoized arm runs the same step as a named recurrence, with
    // Memoizer's cache behind it instead of Eat.
    private static int EatAfterHalvingOrThirding(int orangeCount) => 1 + Math.Min(
        (orangeCount % EatOneAtATimeThenHalveDivisor) + Eat(orangeCount / EatOneAtATimeThenHalveDivisor),
        (orangeCount % EatOneAtATimeThenThirdDivisor) + Eat(orangeCount / EatOneAtATimeThenThirdDivisor));
}
