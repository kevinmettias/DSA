using DSAExperimentation.LeetCode.Solutions.DesignGraphWithShortestPathCalculator;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DesignGraphWithShortestPathCalculator;

public sealed partial class DesignGraphWithShortestPathCalculatorCallsTests
{
    // Edges 0 -> 1 costing 2 and 1 -> 2 costing 3, so 0 reaches 2 for 5 and 2 reaches nothing.
    private static readonly int[][] Edges = [[0, 1, 2], [1, 2, 3]];

    [Fact]
    public void ShortestPath_ReachableNode_IsTheCheapestPathsCost()
    {
        var graph = DesignGraphWithShortestPathCalculatorSolution.CreateByArrayDijkstra(3, Edges);

        Assert.Equal(5, DesignGraphWithShortestPathCalculatorCalls.ShortestPath(graph, 0, 2));
    }

    [Fact]
    public void ShortestPath_UnreachableNode_IsMinusOne()
    {
        var graph = DesignGraphWithShortestPathCalculatorSolution.CreateByArrayDijkstra(3, Edges);

        Assert.Equal(-1, DesignGraphWithShortestPathCalculatorCalls.ShortestPath(graph, 2, 0));
    }
}
