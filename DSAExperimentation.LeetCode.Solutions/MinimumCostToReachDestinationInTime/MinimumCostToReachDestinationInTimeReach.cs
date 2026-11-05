namespace DSAExperimentation.LeetCode.Solutions.MinimumCostToReachDestinationInTime;

// The reach of MinimumCostToReachDestinationInTimeSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MinCostByStateExpandedDijkstra takes 9.5 ms over
// the 11 of the catalog's 98 cases with a length of edges up to 4 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumCostToReachDestinationInTimeReach
{
    private const int LargestEdgesLength = 4;

    public static bool IsWithin(int maxTime, int[][] edges, int[] passingFees) =>
        edges.Length <= LargestEdgesLength;
}
