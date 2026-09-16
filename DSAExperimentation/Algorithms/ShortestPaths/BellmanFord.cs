using System.Numerics;

using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// Relaxes ShortestPath.cs's own non-negative-edge-weight law: Dijkstra/AStar are wrong (not
// merely slow) once a negative weight is reachable, because their "settled" node is assumed
// final the moment it's popped. Bellman-Ford makes no such assumption - it relaxes every edge
// on every round instead of relying on a frontier order - which is exactly what lets it accept
// negative weights and, as a direct consequence of the same round-bound, detect a negative
// cycle rather than loop or mis-answer.
//
// Doesn't share Dijkstra/AStar's Explore/Traverse/RelaxNeighbors engine - there's no priority
// queue, no settled set, no single shared "one algorithm, one injected axis" shape to close
// over (contrast ShortestPath.cs's own AStar-is-Dijkstra-closed-over-a-heuristic framing) - so
// this stays its own file/class rather than a third method on ShortestPath, even though it
// reuses ShortestPath's own IEdgeTopology/IEdges contracts unchanged.
//
// vertices must enumerate the graph's complete vertex set, not merely nodes reachable from
// source - the same precondition shape TopologicalSort.TrySort has for its own `nodes`
// parameter. It bounds relaxation at exactly |V| - 1 rounds and is what makes a further-
// improving round mean "negative cycle," not "graph incomplete." Unlike TrySort, an omitted
// vertex here can only ever under-report: CollectEdges only ever walks `vertices`, so a missing
// vertex just means its own outgoing edges are never considered, never a spurious cycle report.
internal static class BellmanFord
{
    public static bool TryComputeDistances<TNode, TTopology, TEdges, TWeight>(
        IEnumerable<TNode> vertices, TNode source, out Dictionary<TNode, TWeight> distances)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>
    {
        var vertexList = vertices.ToList();
        var edges = CollectEdges<TNode, TTopology, TEdges, TWeight>(vertexList);

        distances = new Dictionary<TNode, TWeight> { [source] = TWeight.Zero };

        for (var round = 0; round < vertexList.Count - 1; round++)
        {
            TryRelaxAll(edges, distances);
        }

        // A distance can still improve on this extra round only if some cycle on the path
        // to it keeps paying off forever - the definition of a negative cycle reachable from
        // source. distances is left exactly as computed through the |V| - 1 guaranteed-valid
        // rounds - a wrong/incomplete answer, not an exception, the same shape TrySort's own
        // false case already establishes.
        return !TryRelaxAll(edges, distances);
    }

    // Returns whether any edge still improved a known distance - true here (on the |V|th call)
    // is the negative-cycle signal; on every earlier call the return value is unused. The `Try`
    // is the attempt-returns-success convention: every round mutates `distances`, and the bool
    // is whether the attempt found anything left to improve.
    private static bool TryRelaxAll<TNode, TWeight>(
        List<(TNode From, TNode To, TWeight Weight)> edges, Dictionary<TNode, TWeight> distances)
        where TNode : class
        where TWeight : INumber<TWeight>
    {
        var improved = false;

        foreach (var (from, to, weight) in edges)
        {
            if (!distances.TryGetValue(from, out var known))
            {
                continue;
            }

            var candidate = known + weight;

            if (!distances.TryGetValue(to, out var existing) || candidate < existing)
            {
                distances[to] = candidate;
                improved = true;
            }
        }

        return improved;
    }

    private static List<(TNode From, TNode To, TWeight Weight)> CollectEdges<TNode, TTopology, TEdges, TWeight>(
        List<TNode> vertices)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
    {
        var edges = new List<(TNode From, TNode To, TWeight Weight)>();

        foreach (var vertex in vertices)
        {
            var neighbors = TTopology.GetEdges(vertex);

            for (var i = 0; i < neighbors.Count; i++)
            {
                var (weight, neighbor) = neighbors.Get(i);
                edges.Add((vertex, neighbor, weight));
            }
        }

        return edges;
    }
}
