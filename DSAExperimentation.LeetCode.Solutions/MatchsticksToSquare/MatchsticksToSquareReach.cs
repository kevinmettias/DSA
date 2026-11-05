namespace DSAExperimentation.LeetCode.Solutions.MatchsticksToSquare;

// The reach of MatchsticksToSquareSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. A backtracking arm's time follows how long it searches before it finds
// a square or rules one out, which length bounds poorly: fifteen sticks of 8, a sum of 120 that no square of
// equal sides can use, take CanMakeSquareByGenericBacktrack 272 ms alone, and a bound on length would keep
// only 16 of the catalog's 74 cases. Sorted by the sum of their sticks instead, the 48 cases up to a sum of
// 114 take it 7.4 ms (measured 2026-11-05, Release); the case past them runs past the 250 ms a strategy's
// catalog cases are allowed. The sum is a measured cut over the catalog's cases, not the arms' cost.
internal static class MatchsticksToSquareReach
{
    private const long LargestStickSum = 114;

    public static bool IsWithin(int[] matchsticks) =>
        matchsticks.Sum(stick => (long)stick) <= LargestStickSum;
}
