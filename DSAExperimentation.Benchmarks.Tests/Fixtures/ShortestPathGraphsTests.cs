using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ShortestPathGraphs (ARCHITECTURE 17.7). The LC 847 reading depends on the
// graph really being connected: the visit-state search explodes with the number of nodes, and a
// graph with an unreachable component would let both arms answer for a smaller graph than the one
// the benchmark claims to measure. The spanning-tree-plus-extras construction is what guarantees it.
public sealed partial class ShortestPathGraphsTests
{
    private const int NodeCount = 8;
    private const int Seed = 847; // LC problem number
    private const int FirstNode = 0;

    [Fact]
    public void BuildRandomConnectedGraph_NodeCount_ReturnsOneAdjacencyListPerNode() =>
        Assert.Equal(
            NodeCount,
            ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed).Length);

    [Fact]
    public void BuildRandomConnectedGraph_EveryNode_IsReachableFromTheFirst() =>
        Assert.Equal(
            NodeCount,
            ReachableCount(ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed)));

    [Fact]
    public void BuildRandomConnectedGraph_EveryEdge_IsRecordedInBothDirections()
    {
        var graph = ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed);

        for (var node = 0; node < graph.Length; node++)
        {
            foreach (var neighbour in graph[node])
            {
                Assert.Contains(node, graph[neighbour]);
            }
        }
    }

    [Fact]
    public void BuildRandomConnectedGraph_EveryAdjacencyList_ExcludesSelfLoopsAndRepeats()
    {
        var graph = ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed);

        for (var node = 0; node < graph.Length; node++)
        {
            Assert.DoesNotContain(node, graph[node]);
            Assert.Equal(graph[node].Length, graph[node].Distinct().Count());
            Assert.All(graph[node], neighbour => Assert.InRange(neighbour, FirstNode, NodeCount - 1));
        }
    }

    [Fact]
    public void BuildRandomConnectedGraph_SameSeed_ReturnsTheSameGraph() =>
        Assert.Equal(
            AnswerText.Of(ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed)),
            AnswerText.Of(ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, Seed)));

    private static int ReachableCount(int[][] graph)
    {
        var reached = new bool[graph.Length];
        var pending = new Queue<int>();
        reached[FirstNode] = true;
        pending.Enqueue(FirstNode);

        while (pending.Count > 0)
        {
            foreach (var neighbour in graph[pending.Dequeue()])
            {
                if (!reached[neighbour])
                {
                    reached[neighbour] = true;
                    pending.Enqueue(neighbour);
                }
            }
        }

        return reached.Count(node => node);
    }
}
