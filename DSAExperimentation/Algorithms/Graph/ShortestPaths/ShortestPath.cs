using System.Numerics;

using DSAExperimentation.Algorithms.Heap;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;

namespace DSAExperimentation.Algorithms.Graph.ShortestPaths;

// Dijkstra and A* are the same priority-queue relaxation loop; the only things that
// vary between them are what a candidate's queue priority is (real distance alone,
// vs real distance plus a heuristic estimate of what's left) and whether there's a
// specific target to stop early at. Both are expressed here as THeuristic plus an
// optional target on one shared Explore core, rather than as two parallel
// implementations - see IPathHeuristic for why that's sound (ZeroHeuristic
// degenerates to exactly Dijkstra's behavior).
//
// Requires every edge weight to be non-negative. Neither Dijkstra nor AStar is correct
// otherwise - a settled node could still have a cheaper path through an edge this
// hasn't relaxed yet - and nothing here checks it; it's on the caller's TEdges.
//
// TWeight uses .NET's generic math (INumber/IMinMaxValue) for +, <, Zero - the same
// static-abstract-member dispatch used everywhere else in this library, just from
// the BCL instead of a hand-rolled interface. The frontier is Collections.Heap's
// Heap<T,TOrder>, ordered by ByPriorityOrder so only .Priority is ever compared.
internal static class ShortestPath
{
    public static Dictionary<TNode, TWeight> Dijkstra<TNode, TTopology, TEdges, TWeight>(TNode source)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
        => Explore<TNode, TTopology, TEdges, TWeight, ZeroHeuristic<TNode, TWeight>>(source, target: null);

    // Only sound when THeuristic is consistent (see IPathHeuristic) - that's exactly
    // what guarantees a node's first pop already carries its true shortest distance,
    // which is what lets Explore stop as soon as target is settled, instead of
    // exhausting the whole graph the way Dijkstra does.
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
        var state = new SearchState<TNode, TWeight>();
        state.Distances[source] = TWeight.Zero;
        var initialPriority = THeuristic.Estimate(source, target);
        state.Queue.Push((source, initialPriority));
        Traverse<TNode, TTopology, TEdges, TWeight, THeuristic>(state, target);

        return state.Distances;
    }

    private static void Traverse<TNode, TTopology, TEdges, TWeight, THeuristic>(
        SearchState<TNode, TWeight> state, TNode? target)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
        where THeuristic : struct, IPathHeuristic<TNode, TWeight>
    {
        while (state.Queue.TryPop(out var entry))
        {
            var node = entry.Node;

            // A node can be pushed more than once (once per relaxation, since Heap has
            // no decrease-key); the first pop is always the true shortest distance, so
            // later stale entries just get skipped.
            if (!state.Settled.Add(node))
            {
                continue;
            }

            if (target is not null && node.Equals(target))
            {
                break;
            }

            RelaxNeighbors<TNode, TTopology, TEdges, TWeight, THeuristic>(node, state.Distances[node], target, state);
        }
    }

    private static void RelaxNeighbors<TNode, TTopology, TEdges, TWeight, THeuristic>(
        TNode node, TWeight distance, TNode? target, SearchState<TNode, TWeight> state)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, TEdges, TWeight>
        where TEdges : struct, IEdges<TNode, TWeight>
        where TWeight : INumber<TWeight>, IMinMaxValue<TWeight>
        where THeuristic : struct, IPathHeuristic<TNode, TWeight>
    {
        var edges = TTopology.GetEdges(node);

        for (var i = 0; i < edges.Count; i++)
        {
            var (weight, neighbor) = edges.Get(i);
            var candidate = distance + weight;

            if (!state.Distances.TryGetValue(neighbor, out var known) || candidate < known)
            {
                state.Distances[neighbor] = candidate;
                var priority = candidate + THeuristic.Estimate(neighbor, target);
                state.Queue.Push((neighbor, priority));
            }
        }
    }

    // Bundles the search's mutable collections so a relaxation step names one state
    // parameter instead of the distance map, the settled set and the frontier queue.
    private sealed record SearchState<TNode, TWeight>
        where TNode : class
        where TWeight : IComparable<TWeight>
    {
        public Dictionary<TNode, TWeight> Distances { get; } = new();
        public HashSet<TNode> Settled { get; } = new();
        public Heap<(TNode Node, TWeight Priority), ByPriorityOrder<TNode, TWeight>> Queue { get; } = new();
    }
}
