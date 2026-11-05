namespace DSAExperimentation.LeetCode.Solutions.ShortestPathVisitingAllNodes;

// The reach of ShortestPathVisitingAllNodesSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. ShortestPathLengthByReduceGraph takes 241.6 ms over the
// 52 of the catalog's 63 cases with an element count of graph up to 38 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class ShortestPathVisitingAllNodesReach
{
    private const int LargestGraphElements = 38;

    public static bool IsWithin(int[][] graph) =>
        graph.Sum(row => row.Length) <= LargestGraphElements;
}
