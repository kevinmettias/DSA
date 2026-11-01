using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

/// <summary>Breadth-first reduce order, entering nodes level by level and never calling Exit.</summary>
public readonly struct BreadthFirstReduceOrder<TNode> : IReduceOrderStrategy<TNode>
    where TNode : class
{
    static TState IReduceOrderStrategy<TNode>.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TState seed, TGuard guard)
        => BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
            root, seed, guard);
}
