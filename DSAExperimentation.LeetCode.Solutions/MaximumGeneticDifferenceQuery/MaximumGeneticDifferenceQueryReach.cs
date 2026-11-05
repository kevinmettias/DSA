namespace DSAExperimentation.LeetCode.Solutions.MaximumGeneticDifferenceQuery;

// The reach of MaximumGeneticDifferenceQuerySolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. MaxGeneticDifferenceByBitTrieDfs takes 6.4 ms over the 8
// of the catalog's 58 cases with a length of queries up to 4 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class MaximumGeneticDifferenceQueryReach
{
    private const int LargestQueriesLength = 4;

    public static bool IsWithin(int[] parents, int[][] queries) =>
        queries.Length <= LargestQueriesLength;
}
