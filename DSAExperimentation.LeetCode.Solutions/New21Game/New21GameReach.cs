namespace DSAExperimentation.LeetCode.Solutions.New21Game;

// The reach of New21GameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. ProbabilityByUnmemoizedRecursion takes 9.1 ms over the 13 of the catalog's
// 63 cases with a value of stopAt up to 20 and a value of maxPts up to 5 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class New21GameReach
{
    private const int LargestStopAt = 20;
    private const int LargestMaxPts = 5;

    public static bool IsWithin(int limit, int stopAt, int maxPts) =>
        stopAt <= LargestStopAt && maxPts <= LargestMaxPts;
}
