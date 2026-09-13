using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumNumberOfDaysToEatNOranges;

// LeetCode 1553. Minimum Number of Days to Eat N Oranges: each day you may eat one
// orange, or half of them when the count is even, or two thirds when it is divisible
// by three. The fewest days is days(n) = n when n <= 1, else
// 1 + min(n % 2 + days(n / 2), n % 3 + days(n / 3)) - eat off the remainder one at a
// time, then spend a single day halving or thirding whatever is left.
//
// Both strategies walk that exact recurrence; the only difference is whether
// repeated states are cached. n stays int, not long: the problem's own stated upper
// bound (2*10^9) is below int.MaxValue, so unlike IntegerReplacement's n+1 branch
// there is no widening to do here.
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
    public static int MinDaysByUnmemoizedRecursion(int n) => Eat(n);

    private static int Eat(int n)
        => n <= 1
            ? n
            : 1 + Math.Min(
                (n % EatOneAtATimeThenHalveDivisor) + Eat(n / EatOneAtATimeThenHalveDivisor),
                (n % EatOneAtATimeThenThirdDivisor) + Eat(n / EatOneAtATimeThenThirdDivisor));

    // This repo's own Memoizer-driven DP recurrence (same IntegerReplacementSolution/
    // FibonacciNumberSolution composition) over the identical recurrence above -
    // caching collapses that reconverging call tree down to the handful of distinct
    // states a chain of halvings and thirdings can actually reach.
    public static int MinDaysByMemoizedRecurrence(int n)
        => Memoizer.Memoize<int, int>(n, (value, days) => value <= 1
            ? value
            : 1 + Math.Min(
                (value % EatOneAtATimeThenHalveDivisor) + days(value / EatOneAtATimeThenHalveDivisor),
                (value % EatOneAtATimeThenThirdDivisor) + days(value / EatOneAtATimeThenThirdDivisor)));
}
