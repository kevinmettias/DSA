namespace DSAExperimentation.LeetCode.Solutions.MinimumOneBitOperationsToMakeIntegersZero;

// The reach of MinimumOneBitOperationsToMakeIntegersZeroSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. MinimumOneBitOperationsByBreadthFirstSearch
// takes 244 ms over the 45 of the catalog's 68 cases with a value of target up to 1,000,000 (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumOneBitOperationsToMakeIntegersZeroReach
{
    private const int LargestTarget = 1_000_000;

    public static bool IsWithin(int target) =>
        target <= LargestTarget;
}
