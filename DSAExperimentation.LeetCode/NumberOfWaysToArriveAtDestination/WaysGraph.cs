using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

// The undirected weighted graph LeetCode's own (n, roads) input describes, with
// every intersection already labelled by its shortest travel time to the
// destination - the one Dijkstra run both counting strategies share
// (RestrictedPathGraph's own framing for LC 1786).
//
// Labelling is part of building the model rather than part of either count: "on a
// shortest path" is stated in terms of these distances, so an intersection without
// its Dist is not yet this problem's graph.
internal sealed class WaysGraph
{
    // The endpoints' and the travel time's slots in LeetCode's own three-element
    // road array.
    private const int From = 0;
    private const int To = 1;
    private const int Time = 2;

    // Intersection 0, where every counted journey begins.
    public WaysNode Start { get; }

    private WaysGraph(WaysNode start) => Start = start;

    public static WaysGraph Build(int intersectionCount, int[][] roads)
    {
        var nodes = BuildNodes(intersectionCount, roads);

        AssignDistancesFromDestination(nodes);

        return new WaysGraph(nodes[0]);
    }

    // LeetCode labels the intersections 0..n-1, so slot i holds intersection i.
    private static WaysNode[] BuildNodes(int intersectionCount, int[][] roads)
    {
        var nodes = new WaysNode[intersectionCount];

        for (var id = 0; id < intersectionCount; id++)
        {
            nodes[id] = new WaysNode(id);
        }

        foreach (var road in roads)
        {
            var (from, to, time) = (road[From], road[To], (long)road[Time]);
            nodes[from].Edges.Add((time, nodes[to]));
            nodes[to].Edges.Add((time, nodes[from]));
        }

        return nodes;
    }

    // Every road is bi-directional, so a single Dijkstra sourced at intersection
    // n - 1 gives every other intersection its distance *to* the destination in one
    // pass. LeetCode guarantees the graph is connected, so every node is reached.
    private static void AssignDistancesFromDestination(WaysNode[] nodes)
    {
        var distances = ShortestPath.Dijkstra<
            WaysNode, WaysEdgeTopology, ListEdges<WaysNode, long>, long>(nodes[^1]);

        foreach (var node in nodes)
        {
            node.Dist = distances[node];
        }
    }
}
