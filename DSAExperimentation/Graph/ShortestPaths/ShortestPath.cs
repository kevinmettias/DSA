using System.Numerics;

namespace DSAExperimentation.Graph;

// Dijkstra and A* are the same priority-queue relaxation loop; the only things that
// vary between them are what a candidate's queue priority is (real distance alone,
// vs real distance plus a heuristic estimate of what's left) and whether there's a
// specific target to stop early at. Both are expressed here as THeuristic plus an
// optional target on one shared Explore core, rather than as two parallel
// implementations - see IPathHeuristic for why that's sound (ZeroHeuristic
// degenerates to exactly Dijkstra's behavior).
//
// TWeight uses .NET's generic math (INumber/IMinMaxValue) for +, <, Zero - the same
// static-abstract-member dispatch used everywhere else in this library, just from
// the BCL instead of a hand-rolled interface.
public static class ShortestPath
{
    public static Dictionary<TNode, TWeight> Dijkstra<TNode, TTopology, TEdges, TWeight>(TNode source)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
        => Explore<TNode, TTopology, TEdges, TWeight, ZeroHeuristic<TNode, TWeight>>(source, target: null);

    // Only sound when THeuristic never overestimates the true remaining distance to
    // target ("admissible") - that admissibility is exactly what lets Explore stop
    // as soon as target is settled, instead of exhausting the whole graph the way
    // Dijkstra does.
    public static TWeight? AStar<TNode, TTopology, TEdges, TWeight, THeuristic>(TNode source, TNode target)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : struct, INumber<TWeight>, IMinMaxValue<TWeight>
        where THeuristic : struct, IPathHeuristic<TNode, TWeight>
    {
        var distances = Explore<TNode, TTopology, TEdges, TWeight, THeuristic>(source, target);

        return distances.TryGetValue(target, out var distance) ? distance : null;
    }

    private static Dictionary<TNode, TWeight> Explore<TNode, TTopology, TEdges, TWeight, THeuristic>(
        TNode source, TNode? target)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
        where THeuristic : struct, IPathHeuristic<TNode, TWeight>
    {
        var distances = new Dictionary<TNode, TWeight> { [source] = TWeight.Zero };
        var settled = new HashSet<TNode>();
        var queue = new PriorityQueue<TNode, TWeight>();
        queue.Enqueue(source, THeuristic.Estimate(source, target));

        while (queue.TryDequeue(out var node, out _))
        {
            // A node can be enqueued more than once (once per relaxation, since
            // PriorityQueue has no decrease-key); the first dequeue is always the
            // true shortest distance, so later stale entries just get skipped.
            if (!settled.Add(node))
            {
                continue;
            }

            if (target is not null && node.Equals(target))
            {
                break;
            }

            var distance = distances[node];
            var edges = TTopology.GetEdges(node);

            for (var i = 0; i < edges.Count; i++)
            {
                var (weight, neighbor) = edges[i];
                var candidate = distance + weight;

                if (!distances.TryGetValue(neighbor, out var known) || candidate < known)
                {
                    distances[neighbor] = candidate;
                    queue.Enqueue(neighbor, candidate + THeuristic.Estimate(neighbor, target));
                }
            }
        }

        return distances;
    }
}
