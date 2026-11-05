namespace DSAExperimentation.LeetCode.Solutions.CountPrimes;

// The reach of CountPrimesSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CountPrimesByTrialDivision takes 220.4 ms over the 20 of the catalog's 30
// cases with a value of limit up to 789,654 (measured 2026-11-05, Release); the larger cases run past the 250
// ms a strategy's catalog cases are allowed.
internal static class CountPrimesReach
{
    private const int LargestLimit = 789_654;

    public static bool IsWithin(int limit) =>
        limit <= LargestLimit;
}
