using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RandomWeightedGraphs (ARCHITECTURE 17.7). The reading depends on the Network Delay
// Time (LC 743) / Cheapest Flights (LC 787) family sharing ONE graph instance: every node i > 0 gets a
// back edge to an earlier node, so node 0 reaches everything and Dijkstra, Bellman-Ford and
// Floyd-Warshall all compare real shortest-path work on identical data.
public sealed partial class RandomWeightedGraphsTests
{
    private const int NodeCount = 50;
    private const int ExtraEdgesPerNode = 3;
    private const int Seed = 42;
    private const int EdgeFieldCount = 3;
    private const int MinEdgeWeight = 1;
    private const int EdgeWeightUpperBound = 50;

    [Fact]
    public void Build_NodeCount_ReturnsOneVertexPerIdInOrderAndNamesNodeZeroAsTheSource()
    {
        var (vertices, source) = RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Equal(NodeCount, vertices.Count);
        Assert.Equal(Enumerable.Range(0, NodeCount), vertices.Select(vertex => vertex.Id));
        Assert.Same(vertices[0], source);
    }

    [Fact]
    public void Build_EveryEdgeWeight_StaysInsideTheDocumentedBand() =>
        Assert.All(
            RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed).Vertices,
            vertex => Assert.All(
                vertex.Edges,
                edge => Assert.InRange(edge.Weight, MinEdgeWeight, EdgeWeightUpperBound - 1)));

    // The back edges are what make node 0 reach every other node, which is the precondition all three
    // shortest-path strategies assume; every edge is stored on its source, so the walk goes over the
    // flat (from, to) pairs rather than the nodes alone.
    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var (vertices, _) = RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed);
        var edges = DirectedEdges(vertices);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge.To == node && edge.From < node);
        }
    }

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowWithDistinctEndpointsInsideTheWeightBand()
    {
        var edges = RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinEdgeWeight, EdgeWeightUpperBound - 1));
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var edges = RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(
            edges.Length,
            NodeCount - 1,
            (NodeCount - 1) + (NodeCount * ExtraEdgesPerNode));
    }

    // The generator states that BuildEdges is the same graph as Build, flattened into LC 2642's own
    // [from, to, weight] shape at the same seed and density - so the two entries are one workload and a
    // drift between them means the two benchmarks are timing two different graphs.
    [Fact]
    public void BuildEdges_AgreesWithBuildOnTheSameSeedAndDensity() =>
        Assert.Equal(
            AnswerText.Of(Flatten(RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed).Vertices)),
            AnswerText.Of(RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));

    [Fact]
    public void Build_SameSeed_ReturnsTheSameGraph() =>
        Assert.Equal(
            AnswerText.Of(Flatten(RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed).Vertices)),
            AnswerText.Of(Flatten(RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, Seed).Vertices)));

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(RandomWeightedGraphs.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));

    // The graph is a record whose Edges list compares by reference, so equality between two builds is
    // read off the flattened edge list rather than off the node objects themselves.
    private static List<int[]> Flatten(List<WeightedGraphNode> vertices) =>
    [
        .. vertices.SelectMany(vertex => vertex.Edges.Select(edge => new[] { vertex.Id, edge.Target.Id, edge.Weight })),
    ];

    private static List<(int From, int To)> DirectedEdges(List<WeightedGraphNode> vertices) =>
    [
        .. vertices.SelectMany(vertex => vertex.Edges.Select(edge => (vertex.Id, edge.Target.Id))),
    ];
}
