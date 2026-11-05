namespace DSAExperimentation.LeetCode.Solutions.DesignGraphWithShortestPathCalculator;

// LeetCode's shortestPath call on the graph, which IShortestPathGraph answers as ShortestPathBetween
// (ARCHITECTURE 17.13).
internal static class DesignGraphWithShortestPathCalculatorCalls
{
    public static int ShortestPath(DesignGraphWithShortestPathCalculatorSolution.IShortestPathGraph graph, int node1, int node2) =>
        graph.ShortestPathBetween(node1, node2);
}
