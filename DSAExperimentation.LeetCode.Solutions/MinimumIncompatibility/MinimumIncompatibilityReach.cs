namespace DSAExperimentation.LeetCode.Solutions.MinimumIncompatibility;

// The reach of MinimumIncompatibilitySolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. MinimumIncompatibilityByMemoizedRecursion takes 42 ms over the 18
// of the catalog's 91 cases with a value of groupCount up to 2 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumIncompatibilityReach
{
    private const int LargestGroupCount = 2;

    public static bool IsWithin(int[] nums, int groupCount) =>
        groupCount <= LargestGroupCount;
}
