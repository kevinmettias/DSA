namespace DSAExperimentation.LeetCode.Solutions.CheckIfPointIsReachable;

// The reach of CheckIfPointIsReachableSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. IsReachableByBruteForceBfs takes 236.1 ms over the 58 of the
// catalog's 85 cases with a value of targetX up to 1,023 and a value of targetY up to 513 (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CheckIfPointIsReachableReach
{
    private const int LargestTargetX = 1023;
    private const int LargestTargetY = 513;

    public static bool IsWithin(int targetX, int targetY) =>
        targetX <= LargestTargetX && targetY <= LargestTargetY;
}
