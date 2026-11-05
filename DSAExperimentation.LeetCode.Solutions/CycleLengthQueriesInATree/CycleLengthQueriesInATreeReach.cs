namespace DSAExperimentation.LeetCode.Solutions.CycleLengthQueriesInATree;

// The reach of CycleLengthQueriesInATreeSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CycleLengthQueriesByAncestorDictionary takes 1.7 ms over the 67
// of the catalog's 69 cases with a length of queries up to 29 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CycleLengthQueriesInATreeReach
{
    private const int LargestQueriesLength = 29;

    public static bool IsWithin(int treeLevels, int[][] queries) =>
        queries.Length <= LargestQueriesLength;
}
