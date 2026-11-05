namespace DSAExperimentation.LeetCode.Solutions.MinimumCostTreeFromLeafValues;

// The reach of MinimumCostTreeFromLeafValuesSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. MinimumCostTreeFromLeafValuesByUnmemoizedRecursion takes
// 115 ms over the 72 of the catalog's 90 cases with a length of arr up to 14 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumCostTreeFromLeafValuesReach
{
    private const int LargestArrLength = 14;

    public static bool IsWithin(int[] arr) =>
        arr.Length <= LargestArrLength;
}
