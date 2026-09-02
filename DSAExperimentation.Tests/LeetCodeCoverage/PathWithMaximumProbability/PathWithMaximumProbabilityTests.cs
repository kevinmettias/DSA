using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.PathWithMaximumProbability.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMaximumProbability;

// LeetCode 1514. Path with Maximum Probability: maximizing a product of edge
// probabilities along undirected weighted edges is the same problem as
// minimizing the sum of each edge's -log(probability) - a non-negative
// transform (a probability in (0,1] has log <= 0) that turns this into
// exactly this repo's own ShortestPath.Dijkstra precondition (non-negative
// edge weights, minimize distance), with zero changes to Dijkstra itself.
// The answer is Math.Exp(-distance): undoing the log transform.
public sealed partial class PathWithMaximumProbabilityTests
{
    [Fact]
    public void MaxProbability_LeetCodeExampleOne_PrefersHigherProductTwoHopPath()
    {
        var probability = MaxProbability(
            new ProbabilityGraph(NodeCount: 3, Edges: [[0, 1], [1, 2], [0, 2]], SuccessProbabilities: [0.5, 0.5, 0.2]), start: 0, end: 2);

        Assert.Equal(0.25, probability, precision: 5);
    }

    [Fact]
    public void MaxProbability_LeetCodeExampleTwo_PrefersDirectEdgeOverLongerPath()
    {
        var probability = MaxProbability(
            new ProbabilityGraph(NodeCount: 3, Edges: [[0, 1], [1, 2], [0, 2]], SuccessProbabilities: [0.5, 0.5, 0.3]), start: 0, end: 2);

        Assert.Equal(0.3, probability, precision: 5);
    }

    [Fact]
    public void MaxProbability_LeetCodeExampleThree_UnreachableTargetReturnsZero()
    {
        var probability = MaxProbability(new ProbabilityGraph(NodeCount: 3, Edges: [[0, 1]], SuccessProbabilities: [0.5]), start: 0, end: 2);

        Assert.Equal(0.0, probability, precision: 5);
    }

    private readonly record struct ProbabilityGraph(int NodeCount, int[][] Edges, double[] SuccessProbabilities);

    private static double MaxProbability(ProbabilityGraph graph, int start, int end)
    {
        var nodes = Enumerable.Range(0, graph.NodeCount).Select(id => new ProbabilityNode(id)).ToArray();

        for (var i = 0; i < graph.Edges.Length; i++)
        {
            var (a, b) = (graph.Edges[i][0], graph.Edges[i][1]);
            var cost = -Math.Log(graph.SuccessProbabilities[i]);
            nodes[a].Edges.Add((cost, nodes[b]));
            nodes[b].Edges.Add((cost, nodes[a]));
        }

        var distances = ShortestPath.Dijkstra<
            ProbabilityNode, ProbabilityTopology, ListEdges<ProbabilityNode, double>, double>(nodes[start]);

        return distances.TryGetValue(nodes[end], out var cost2) ? Math.Exp(-cost2) : 0.0;
    }
}
