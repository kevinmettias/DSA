namespace DSAExperimentation.LeetCode.Solutions.SelectCellsInGridWithMaximumScore;

// The reach of SelectCellsInGridWithMaximumScoreSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. MaxScoreByBruteForceRecursion takes 49.1 ms over the 87
// of the catalog's 89 cases with an element count of grid up to 45 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class SelectCellsInGridWithMaximumScoreReach
{
    private const int LargestGridElements = 45;

    public static bool IsWithin(int[][] grid) =>
        grid.Sum(row => row.Length) <= LargestGridElements;
}
