namespace DSAExperimentation.LeetCode.Solutions.MinimumScoreTriangulationOfPolygon;

// The reach of MinimumScoreTriangulationOfPolygonSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MinScoreTriangulationByUnmemoizedRecursion takes
// 71.9 ms over the 74 of the catalog's 93 cases with a length of values up to 16 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumScoreTriangulationOfPolygonReach
{
    private const int LargestValuesLength = 16;

    public static bool IsWithin(int[] values) =>
        values.Length <= LargestValuesLength;
}
