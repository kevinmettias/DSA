using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

/// <summary>Depth-first reduce order, entering each node before its subtree and exiting it after.</summary>
public readonly struct DepthFirstReduceOrder<TNode> : IReduceOrderStrategy<TNode>
    where TNode : class
{
    static TState IReduceOrderStrategy<TNode>.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TState seed, TGuard guard)
        => DepthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
            root, 0, seed, guard);
}
