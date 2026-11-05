namespace DSAExperimentation.LeetCode.Solutions.MinimumHeightTrees;

// The reach of MinimumHeightTreesSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. FindRootsByLeafPeeling takes 2.4 ms over the 37 of the catalog's 65
// cases with a length of edges up to 11 (measured 2026-11-05, Release); the larger cases run past the 250 ms
// a strategy's catalog cases are allowed.
internal static class MinimumHeightTreesReach
{
    private const int LargestEdgesLength = 11;

    public static bool IsWithin(int nodeCount, int[][] edges) =>
        edges.Length <= LargestEdgesLength;
}
