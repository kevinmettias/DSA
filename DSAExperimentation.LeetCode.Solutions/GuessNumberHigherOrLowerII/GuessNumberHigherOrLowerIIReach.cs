namespace DSAExperimentation.LeetCode.Solutions.GuessNumberHigherOrLowerII;

// The reach of GuessNumberHigherOrLowerIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. GetMoneyAmountByUnmemoizedRecursion takes 64.6 ms over the 8 of
// the catalog's 28 cases with a value of highestNumber up to 15 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class GuessNumberHigherOrLowerIIReach
{
    private const int LargestHighestNumber = 15;

    public static bool IsWithin(int highestNumber) =>
        highestNumber <= LargestHighestNumber;
}
