using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator;

// LeetCode 2642. Design Graph With Shortest Path Calculator: a directed,
// non-negative-weight graph over nodeCount fixed vertices that gains edges after
// construction and answers repeated shortest-path queries between two given
// nodes, reporting -1 when node2 is unreachable from node1.
//
// AddEdge can change the graph between any two queries, so nothing may be cached:
// every query runs a fresh single-source search from node1 and reads off node2.
// That makes the whole problem a comparison between two ways of running that
// search, which is what the two strategies below are.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md 17.3) takes the form of two full classes
// implementing the shared IShortestPathGraph surface below, the same shape
// DesignBrowserHistorySolution uses for its own instance-API problem (LC 1472).
//
// The query member is named ShortestPathBetween rather than LeetCode's literal
// shortestPath so it does not shadow the imported Algorithms.ShortestPaths.
// ShortestPath static class from inside its own body - an instance member and an
// imported type sharing one bare name in the same class is the C# name-resolution
// wrinkle ARCHITECTURE.md 10.3 documents for DisjointSet, avoided here by naming
// rather than aliasing.
internal static class DesignGraphWithShortestPathCalculatorSolution
{
    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it.
    internal interface IShortestPathGraph
    {
        void AddEdge(int[] edge);

        int ShortestPathBetween(int node1, int node2);
    }

    // The textbook baseline this composition has to justify itself against: an
    // adjacency array plus the O(V^2) form of Dijkstra - a plain distance array
    // and a linear scan for the next unsettled minimum, with no priority queue at
    // all. Deliberately written without this repo's primitives.
    internal sealed class ShortestPathGraphByArrayDijkstra : IShortestPathGraph
    {
        // "the linear scan found nothing left to settle" - an index sentinel,
        // deliberately not LeetCodeAnswer.None, which reports an answer.
        private const int NoUnsettledNode = -1;

        private readonly List<(int Weight, int Target)>[] _adjacency;

        public ShortestPathGraphByArrayDijkstra(int nodeCount, int[][] edges)
        {
            _adjacency = new List<(int Weight, int Target)>[nodeCount];

            for (var i = 0; i < nodeCount; i++)
            {
                _adjacency[i] = [];
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void AddEdge(int[] edge) => _adjacency[edge[0]].Add((edge[2], edge[1]));

        public int ShortestPathBetween(int node1, int node2)
        {
            var distance = Distances(node1)[node2];

            return distance == int.MaxValue ? LeetCodeAnswer.None : distance;
        }

        private int[] Distances(int source)
        {
            var distances = new int[_adjacency.Length];
            Array.Fill(distances, int.MaxValue);
            distances[source] = 0;
            var settled = new bool[_adjacency.Length];

            for (var iteration = 0; iteration < _adjacency.Length; iteration++)
            {
                var current = ExtractMinUnsettled(distances, settled);

                if (current == NoUnsettledNode)
                {
                    break;
                }

                settled[current] = true;
                RelaxNeighbors(distances, current);
            }

            return distances;
        }

        private static int ExtractMinUnsettled(int[] distances, bool[] settled)
        {
            var best = NoUnsettledNode;

            for (var i = 0; i < distances.Length; i++)
            {
                if (settled[i] || distances[i] == int.MaxValue)
                {
                    continue;
                }

                if (best == NoUnsettledNode || distances[i] < distances[best])
                {
                    best = i;
                }
            }

            return best;
        }

        private void RelaxNeighbors(int[] distances, int current)
        {
            foreach (var (weight, target) in _adjacency[current])
            {
                var candidate = distances[current] + weight;

                if (candidate < distances[target])
                {
                    distances[target] = candidate;
                }
            }
        }
    }

    // The composed answer: this repo's own ShortestPath.Dijkstra, whose frontier
    // is Collections.Heap's Heap<T,TOrder>, over a mutable per-node edge list -
    // so AddEdge is just another List<T>.Add and the query is O((V + E) log V)
    // instead of O(V^2). Same WeightedNode/ListEdges/IEdgeTopology composition
    // NetworkDelayTimeSolution uses for LC 743's own single-source distances.
    internal sealed class ShortestPathGraphByHeapDijkstra : IShortestPathGraph
    {
        private readonly ShortestPathCalculatorNode[] _nodes;

        public ShortestPathGraphByHeapDijkstra(int nodeCount, int[][] edges)
        {
            _nodes = new ShortestPathCalculatorNode[nodeCount];

            for (var i = 0; i < nodeCount; i++)
            {
                _nodes[i] = new ShortestPathCalculatorNode(i);
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void AddEdge(int[] edge) => _nodes[edge[0]].Edges.Add((edge[2], _nodes[edge[1]]));

        public int ShortestPathBetween(int node1, int node2)
        {
            var distances = ShortestPath.Dijkstra<
                ShortestPathCalculatorNode,
                ShortestPathCalculatorTopology,
                ListEdges<ShortestPathCalculatorNode, int>,
                int>(_nodes[node1]);

            return distances.TryGetValue(_nodes[node2], out var distance) ? distance : LeetCodeAnswer.None;
        }
    }
}
