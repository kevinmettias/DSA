using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

// Each entry point reads the algebra's StopsEarly once and picks the walk's stop check from it
// (see IStopCheck), so an algebra that never stops gets the walk with no check in it.
/// <summary>Depth-first reduce order, entering each node before its subtree and exiting it after.</summary>
public readonly struct DepthFirstReduceOrder<TNode> : IReduceOrderStrategy<TNode>
    where TNode : class
{
    static TState IReduceOrderStrategy<TNode>.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return DepthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, AlgebraStopCheck>(root, 0, seed, guard);
        }

        return DepthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, NoStopCheck>(root, 0, seed, guard);
    }

    static TState IReduceOrderStrategy<TNode>.EvaluateFrom<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        IEnumerable<TNode> roots, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return DepthFirstWalk.WalkEach<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, AlgebraStopCheck>(roots, seed, guard);
        }

        return DepthFirstWalk.WalkEach<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, NoStopCheck>(roots, seed, guard);
    }
}
