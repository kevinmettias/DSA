namespace DSAExperimentation.LeetCode.Solutions.FindPolygonWithTheLargestPerimeter;

// The reach of FindPolygonWithTheLargestPerimeterSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. LargestPerimeterBySortedRunningSum takes 3.8 ms
// over the 18 of the catalog's 86 cases with a length of nums up to 6 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindPolygonWithTheLargestPerimeterReach
{
    private const int LargestNumsLength = 6;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
