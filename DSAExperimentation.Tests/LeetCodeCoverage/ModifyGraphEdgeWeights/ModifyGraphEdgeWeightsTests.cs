using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ModifyGraphEdgeWeights;

// LeetCode 2699. Modify Graph Edge Weights: assign a positive weight to every -1
// edge so the shortest source->destination distance becomes exactly target. This is
// ShortestPath.Dijkstra called repeatedly over a mutable WeightedNode graph (the
// same fixture NetworkDelayTimeTests already uses) - once with every -1 edge at the
// floor weight 1, then once more per remaining -1 edge, stretching it by exactly
// (target minus the two already-settled half-distances either side of it) until the
// floor reaches target exactly, or every -1 edge is spent without reaching it.
public sealed partial class ModifyGraphEdgeWeightsTests
{
    [Fact]
    public void ModifyEdgeWeights_FloorBelowTarget_StretchesTheNegativeEdgeToHitTarget()
    {
        int[][] edges = [[0, 1, -1], [1, 2, 4]];

        var modified = ModifyEdgeWeights(n: 3, edges, source: 0, destination: 2, target: 10);

        AssertShortestPathEquals(modified, n: 3, source: 0, destination: 2, target: 10);
    }

    [Fact]
    public void ModifyEdgeWeights_FloorAlreadyMatchesTarget_LeavesNegativeEdgeAtMinimumWeight()
    {
        int[][] edges = [[0, 1, -1], [1, 2, 1]];

        var modified = ModifyEdgeWeights(n: 3, edges, source: 0, destination: 2, target: 2);

        AssertShortestPathEquals(modified, n: 3, source: 0, destination: 2, target: 2);
        Assert.Equal(1, modified[0][2]);
    }

    [Fact]
    public void ModifyEdgeWeights_FloorAboveTarget_ReturnsEmptyArray()
    {
        int[][] edges = [[0, 1, -1], [0, 2, 3]];

        var modified = ModifyEdgeWeights(n: 3, edges, source: 0, destination: 2, target: 2);

        Assert.Empty(modified);
    }

    [Fact]
    public void ModifyEdgeWeights_MultipleNegativeEdges_StretchesOnlyAsManyAsNeeded()
    {
        int[][] edges = [[4, 1, -1], [2, 0, -1], [0, 3, -1], [4, 3, -1]];

        var modified = ModifyEdgeWeights(n: 5, edges, source: 0, destination: 1, target: 5);

        AssertShortestPathEquals(modified, n: 5, source: 0, destination: 1, target: 5);
    }

    private static void AssertShortestPathEquals(int[][] edges, int n, int source, int destination, int target)
    {
        Assert.NotEmpty(edges);
        Assert.All(edges, edge => Assert.True(edge[2] >= 1));

        var (nodes, _) = BuildGraph(n, edges);
        var distances = Distances(nodes, source);

        Assert.True(distances.TryGetValue(nodes[destination], out var distance));
        Assert.Equal(target, distance);
    }

    private static int[][] ModifyEdgeWeights(int n, int[][] edges, int source, int destination, int target)
    {
        var (nodes, edgeRefs) = BuildGraph(n, edges);
        var floor = Distances(nodes, source);

        if (!floor.TryGetValue(nodes[destination], out var minDistance) || minDistance > target)
        {
            return [];
        }

        if (minDistance == target)
        {
            return CurrentWeights(edges, edgeRefs, nodes);
        }

        for (var i = 0; i < edges.Length; i++)
        {
            if (edges[i][2] != -1)
            {
                continue;
            }

            SetEdgeWeight(nodes, edgeRefs[i], CandidateWeight(nodes, edgeRefs[i], source, destination, target));

            if (Distances(nodes, source).TryGetValue(nodes[destination], out var distance) && distance == target)
            {
                return CurrentWeights(edges, edgeRefs, nodes);
            }
        }

        return [];
    }

    // Half-distance either side of the edge, from the two settled Dijkstra runs
    // already required above - not re-derived some other way.
    private static int CandidateWeight(
        WeightedNode[] nodes, (int U, int UIndex, int V, int VIndex) edgeRef, int source, int destination, int target)
    {
        var fromSource = Distances(nodes, source);
        var fromDestination = Distances(nodes, destination);

        if (!fromSource.TryGetValue(nodes[edgeRef.U], out var distanceFromSource) ||
            !fromDestination.TryGetValue(nodes[edgeRef.V], out var distanceFromDestination))
        {
            return 1;
        }

        var candidate = target - distanceFromSource - distanceFromDestination;
        return candidate >= 1 ? candidate : 1;
    }

    private static Dictionary<WeightedNode, int> Distances(WeightedNode[] nodes, int source)
        => ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(nodes[source]);

    // Every -1 edge starts at the floor weight 1 - both directions get the same List
    // index recorded so a later SetEdgeWeight can update them together.
    private static (WeightedNode[] Nodes, (int U, int UIndex, int V, int VIndex)[] EdgeRefs) BuildGraph(
        int n, int[][] edges)
    {
        var nodes = new WeightedNode[n];
        for (var i = 0; i < n; i++)
        {
            nodes[i] = new WeightedNode(i.ToString());
        }

        var edgeRefs = new (int, int, int, int)[edges.Length];
        for (var i = 0; i < edges.Length; i++)
        {
            var (u, v, weight) = (edges[i][0], edges[i][1], edges[i][2] == -1 ? 1 : edges[i][2]);
            var uIndex = nodes[u].Edges.Count;
            nodes[u].Edges.Add((weight, nodes[v]));
            var vIndex = nodes[v].Edges.Count;
            nodes[v].Edges.Add((weight, nodes[u]));
            edgeRefs[i] = (u, uIndex, v, vIndex);
        }

        return (nodes, edgeRefs);
    }

    private static void SetEdgeWeight(
        WeightedNode[] nodes, (int U, int UIndex, int V, int VIndex) edgeRef, int weight)
    {
        nodes[edgeRef.U].Edges[edgeRef.UIndex] = (weight, nodes[edgeRef.V]);
        nodes[edgeRef.V].Edges[edgeRef.VIndex] = (weight, nodes[edgeRef.U]);
    }

    private static int[][] CurrentWeights(
        int[][] edges, (int U, int UIndex, int V, int VIndex)[] edgeRefs, WeightedNode[] nodes)
    {
        var result = new int[edges.Length][];
        for (var i = 0; i < edges.Length; i++)
        {
            result[i] = [edges[i][0], edges[i][1], nodes[edgeRefs[i].U].Edges[edgeRefs[i].UIndex].Weight];
        }

        return result;
    }
}
