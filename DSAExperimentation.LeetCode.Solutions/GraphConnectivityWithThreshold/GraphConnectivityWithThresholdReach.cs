namespace DSAExperimentation.LeetCode.Solutions.GraphConnectivityWithThreshold;

// The reach of GraphConnectivityWithThresholdSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. AreConnectedByNaiveUnionFind takes 242.3 ms over the 51
// of the catalog's 63 cases with a value of cityCount up to 8,000 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class GraphConnectivityWithThresholdReach
{
    private const int LargestCityCount = 8000;

    public static bool IsWithin(int cityCount, int threshold, int[][] queries) =>
        cityCount <= LargestCityCount;
}
