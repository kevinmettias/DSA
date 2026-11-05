namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReachAPositionAfterExactlyKSteps;

// The reach of NumberOfWaysToReachAPositionAfterExactlyKStepsSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. NumberOfWaysByMemoizedRecursion takes 3.6
// ms over the 11 of the catalog's 21 cases with a value of stepCount up to 15 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfWaysToReachAPositionAfterExactlyKStepsReach
{
    private const int LargestStepCount = 15;

    public static bool IsWithin(int startPos, int endPos, int stepCount) =>
        stepCount <= LargestStepCount;
}
