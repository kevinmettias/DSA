using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode;

// LeetCode 1786. Number of Restricted Paths From First to Last Node: this repo's
// own ShortestPath.Dijkstra computes every node's distance to node n, then
// DagFold.Fold counts paths from node 1 that only ever step to a strictly-closer
// neighbor. The "restricted" rule turns the graph into a DAG by construction - a
// walk that only ever decreases Dist can never revisit a node - which is exactly
// what unlocks DagFold's memoized fold instead of CheckedFold's runtime cycle
// defense (see RestrictedPathChildTopology).
public sealed partial class NumberOfRestrictedPathsFromFirstToLastNodeTests
{
    [Fact]
    public void CountRestrictedPaths_ClassicExample_ReturnsThree()
    {
        int[][] edges = [[1, 2, 3], [1, 3, 3], [2, 3, 1], [1, 4, 2], [5, 2, 2], [3, 5, 1], [5, 4, 10]];

        var count = CountRestrictedPaths(n: 5, edges);

        Assert.Equal(3, count);
    }

    [Fact]
    public void CountRestrictedPaths_SingleEdge_ReturnsOne()
    {
        int[][] edges = [[1, 2, 1]];

        var count = CountRestrictedPaths(n: 2, edges);

        Assert.Equal(1, count);
    }

    private static long CountRestrictedPaths(int n, int[][] edges)
    {
        var nodes = BuildNodes(n, edges);
        AssignDistancesFromDestination(nodes, n);

        return DagFold.Fold<
            RestrictedPathNode, RestrictedPathChildTopology, ListChildren<RestrictedPathNode>,
            NaturalChildOrder<RestrictedPathNode, ListChildren<RestrictedPathNode>>, ListChildren<RestrictedPathNode>,
            RestrictedPathCountAlgebra, long>(nodes[1]);
    }

    private static Dictionary<int, RestrictedPathNode> BuildNodes(int n, int[][] edges)
    {
        var nodes = new Dictionary<int, RestrictedPathNode>();

        for (var id = 1; id <= n; id++)
        {
            nodes[id] = new RestrictedPathNode(id);
        }

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[0], edge[1], edge[2]);
            nodes[from].Edges.Add((weight, nodes[to]));
            nodes[to].Edges.Add((weight, nodes[from]));
        }

        return nodes;
    }

    private static void AssignDistancesFromDestination(Dictionary<int, RestrictedPathNode> nodes, int destinationId)
    {
        var distances = ShortestPath.Dijkstra<
            RestrictedPathNode, RestrictedPathEdgeTopology, ListEdges<RestrictedPathNode, int>, int>(nodes[destinationId]);

        foreach (var node in nodes.Values)
        {
            node.Dist = distances[node];
        }
    }
}
