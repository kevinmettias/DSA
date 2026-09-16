using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FunctionalGraphs (ARCHITECTURE 17.7). The reading depends on the graph being
// one single cycle through every node: a walk from any start has to traverse the whole cycle before
// it revisits a node, which is the worst case both the naive walk and Tarjan's pass are measured on.
public sealed partial class FunctionalGraphsTests
{
    private const int NodeCount = 64;
    private const int FirstNode = 0;

    [Fact]
    public void BuildSingleCycleEdges_EverySuccessor_StaysInRangeAndIsClaimedExactlyOnce()
    {
        var edges = FunctionalGraphs.BuildSingleCycleEdges(NodeCount);

        Assert.Equal(NodeCount, edges.Length);
        Assert.All(edges, successor => Assert.InRange(successor, FirstNode, NodeCount - 1));
        Assert.Equal(NodeCount, edges.Distinct().Count());
    }

    [Fact]
    public void BuildSingleCycleEdges_WalkingFromTheFirstNode_VisitsEveryNodeOnceAndReturnsToIt()
    {
        var edges = FunctionalGraphs.BuildSingleCycleEdges(NodeCount);
        var visited = Walk(edges, FirstNode);

        Assert.Equal(NodeCount, visited.Count);
        Assert.Equal(NodeCount, visited.Distinct().Count());
        Assert.Equal(FirstNode, edges[visited[^1]]);
    }

    [Fact]
    public void BuildSingleCycleEdges_EveryNode_PointsAtTheNextNodeWrappingAtTheEnd()
    {
        var edges = FunctionalGraphs.BuildSingleCycleEdges(NodeCount);

        Assert.All(
            Enumerable.Range(0, NodeCount),
            node => Assert.Equal((node + 1) % NodeCount, edges[node]));
    }

    private static List<int> Walk(int[] edges, int start)
    {
        var visited = new List<int>(edges.Length);
        var current = start;

        for (var step = 0; step < edges.Length; step++)
        {
            visited.Add(current);
            current = edges[current];
        }

        return visited;
    }
}
