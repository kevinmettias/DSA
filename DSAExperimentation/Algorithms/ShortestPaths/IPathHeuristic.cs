using System.Numerics;

namespace DSAExperimentation.Algorithms.ShortestPaths;

// The single axis ShortestPath.AStar generalizes Dijkstra on: what a candidate's
// queue priority is, beyond its real accumulated distance. ZeroHeuristic makes
// priority == real distance, which is exactly Dijkstra's relaxation order.
//
// Must be CONSISTENT - h(u) <= cost(u,v) + h(v) on every edge u->v, and h(target) == 0
// - not merely admissible (never overestimates). Consistency is strictly stronger, and
// it's the specific property this codebase's algorithm leans on: ShortestPath.Traverse
// settles a node once (Settled.Add) and never reconsiders it, which is only correct
// because consistency guarantees a node's first pop already carries its true shortest
// distance. An admissible-but-inconsistent heuristic can still bias the search toward
// target, but on some graph shapes can also make Traverse settle a node at a
// non-optimal distance with nothing here to catch it - the fix for that would be
// reopening settled nodes, which is not what this contract or this algorithm do. Every
// heuristic in this codebase (ZeroHeuristic, ManhattanHeuristic) is consistent.
/// <summary>
/// An estimate of the distance left to a search's target, which <see cref="ShortestPath"/> adds to a node's
/// real distance to rank it.
/// </summary>
public interface IPathHeuristic<TNode, TWeight>
    where TNode : class
    where TWeight : INumber<TWeight>
{
    /// <summary>Estimates the remaining distance from a node to the search's target.</summary>
    /// <param name="node">The node whose remaining distance is estimated.</param>
    /// <param name="target">
    /// The node the search stops at, or <c>null</c> when it has none and explores everything reachable.
    /// </param>
    /// <returns>
    /// An estimate that is zero at <paramref name="target"/> and never exceeds any outgoing edge's weight
    /// plus the estimate at that edge's far end.
    /// </returns>
    static abstract TWeight Estimate(TNode node, TNode? target);
}
