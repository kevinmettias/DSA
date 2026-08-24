using System.Numerics;

namespace DSAExperimentation.Graph;

// The default IPathHeuristic - every candidate's priority equals its real
// accumulated distance, so ShortestPath.Explore under this heuristic *is* Dijkstra,
// not an approximation of it.
public readonly struct ZeroHeuristic<TNode, TWeight> : IPathHeuristic<TNode, TWeight>
    where TNode : class
    where TWeight : INumber<TWeight>
{
    public static TWeight Estimate(TNode node, TNode? target) => TWeight.Zero;
}
