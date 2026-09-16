using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for KConnectedComponentsWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3608's graph being a simple graph of distinct undirected edges whose times are exactly the
// insertion order, dense enough that removing edges by time is a real component-splitting sweep.
public sealed partial class KConnectedComponentsWorkloadsTests
{
    private const int NodeCount = 32;
    private const int Seed = 3608; // LC problem number
    private const int EdgeFieldCount = 3; // U, V, Time
    private const int FirstTime = 1;
    private const int FewestComponents = 1;
    private const int ComponentCountDivisor = 4;

    [Fact]
    public void BuildEdges_EveryEdge_IsATwoEndpointPairWithAnInRangeTime()
    {
        var edges = KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[2], FirstTime, edges.Length));
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysAtTheDocumentedDensityCap() =>
        Assert.Equal(
            Math.Min(NodeCount * 2, NodeCount * (NodeCount - 1) / 2),
            KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed).Length);

    // The generator rejects a pair already claimed, so no edge may appear twice however its two
    // endpoints were written - the graph is simple, which is what makes the removal sweep's answer
    // well defined.
    [Fact]
    public void BuildEdges_EveryEdge_IsDistinctEvenWhenItsEndpointsSwapOrientation()
    {
        var edges = KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed);

        Assert.Equal(edges.Length, edges.Select(Endpoints).Distinct().Count());
    }

    [Fact]
    public void BuildEdges_EveryTime_CountsUpInInsertionOrder()
    {
        var edges = KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed);

        Assert.Equal(Enumerable.Range(FirstTime, edges.Length), edges.Select(edge => edge[2]));
    }

    // How dense this random graph comes out is not fixed by the seed alone in principle, so what is
    // asserted is the bounded shape the reading needs: the node set starts out in a handful of
    // components, never as a scattering of isolated nodes.
    [Fact]
    public void BuildEdges_Graph_StartsOutInAFewComponents()
    {
        var edges = KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed);

        Assert.InRange(ComponentCount(edges), FewestComponents, NodeCount / ComponentCountDivisor);
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameGraph() =>
        Assert.Equal(
            AnswerText.Of(KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed)),
            AnswerText.Of(KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed)));

    private static (int Low, int High) Endpoints(int[] edge) =>
        edge[0] < edge[1] ? (edge[0], edge[1]) : (edge[1], edge[0]);

    private static int ComponentCount(int[][] edges)
    {
        var neighbours = Enumerable.Range(0, NodeCount).ToDictionary(node => node, _ => new HashSet<int>());

        foreach (var edge in edges)
        {
            neighbours[edge[0]].Add(edge[1]);
            neighbours[edge[1]].Add(edge[0]);
        }

        var componentCount = 0;
        var visited = new HashSet<int>();

        foreach (var node in Enumerable.Range(0, NodeCount))
        {
            if (visited.Add(node))
            {
                componentCount++;
                Visit(neighbours, node, visited);
            }
        }

        return componentCount;
    }

    private static void Visit(Dictionary<int, HashSet<int>> neighbours, int start, HashSet<int> visited)
    {
        var pending = new Queue<int>();
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            foreach (var neighbour in neighbours[pending.Dequeue()])
            {
                if (visited.Add(neighbour))
                {
                    pending.Enqueue(neighbour);
                }
            }
        }
    }
}
