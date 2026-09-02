using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignGraphWithShortestPathCalculator;

// LeetCode 2642. Design Graph With Shortest Path Calculator: a directed,
// non-negative-weight graph that supports adding edges after construction and
// answering repeated shortest-path queries between two given nodes. AddEdge can
// change the graph between calls, so nothing may be cached - each query runs a
// fresh ShortestPath.Dijkstra from node1, the same WeightedNode/ListEdges/
// IEdgeTopology composition NetworkDelayTimeTests already establishes for LC 743's
// own single-source distances-from-source map, here with a mutable per-node edge
// list so AddEdge is just another List<T>.Add. The query method is named
// ShortestPathBetween rather than LeetCode's literal ShortestPath to avoid
// shadowing the imported Algorithms.ShortestPaths.ShortestPath static class from
// inside its own body (an instance member and an imported type sharing one bare
// name inside the same class - the same C# name-resolution wrinkle
// ARCHITECTURE.md 10.3 already documents for DisjointSet, here avoided by naming
// instead of aliasing).
public sealed partial class DesignGraphWithShortestPathCalculatorTests
{
    [Fact]
    public void ShortestPathBetween_GrowingGraph_TracksCheaperRoutesAsEdgesAreAdded()
    {
        int[][] edges = [[0, 2, 5]];
        var graph = new Graph(4, edges);

        Assert.Equal(5, graph.ShortestPathBetween(0, 2));
        Assert.Equal(-1, graph.ShortestPathBetween(0, 3));

        graph.AddEdge([2, 3, 2]);

        Assert.Equal(7, graph.ShortestPathBetween(0, 3));

        graph.AddEdge([0, 1, 1]);
        graph.AddEdge([1, 2, 1]);

        Assert.Equal(2, graph.ShortestPathBetween(0, 2));
        Assert.Equal(4, graph.ShortestPathBetween(0, 3));
    }

    [Fact]
    public void ShortestPathBetween_NoEdgesAtAll_ReturnsNegativeOneExceptSameNode()
    {
        var graph = new Graph(2, []);

        Assert.Equal(0, graph.ShortestPathBetween(0, 0));
        Assert.Equal(-1, graph.ShortestPathBetween(0, 1));
    }

    private sealed class Graph
    {
        private readonly Node[] _nodes;

        public Graph(int n, int[][] edges)
        {
            _nodes = new Node[n];

            for (var i = 0; i < n; i++)
            {
                _nodes[i] = new Node();
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void AddEdge(int[] edge) => _nodes[edge[0]].Edges.Add((edge[2], _nodes[edge[1]]));

        public int ShortestPathBetween(int node1, int node2)
        {
            var distances = ShortestPath.Dijkstra<Node, NodeTopology, ListEdges<Node, int>, int>(_nodes[node1]);

            return distances.TryGetValue(_nodes[node2], out var distance) ? distance : -1;
        }

        private sealed class Node
        {
            public List<(int Weight, Node Target)> Edges { get; } = [];
        }

        private readonly struct NodeTopology : IEdgeTopology<Node, ListEdges<Node, int>, int>
        {
            public static ListEdges<Node, int> GetEdges(Node node) => new(node.Edges);
        }
    }
}
