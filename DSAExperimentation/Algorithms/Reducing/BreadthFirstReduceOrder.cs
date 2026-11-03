using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

// Each entry point picks the walk's stop check from the algebra's StopsEarly, as
// DepthFirstReduceOrder does.
/// <summary>Breadth-first reduce order, entering nodes level by level and never calling Exit.</summary>
public readonly struct BreadthFirstReduceOrder<TNode> : IReduceOrderStrategy<TNode>
    where TNode : class
{
    static TState IReduceOrderStrategy<TNode>.Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, AlgebraStopCheck>(root, seed, guard);
        }

        return BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, NoStopCheck>(root, seed, guard);
    }

    static TState IReduceOrderStrategy<TNode>.EvaluateFrom<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        IEnumerable<TNode> roots, TState seed, TGuard guard)
    {
        if (TAlgebra.StopsEarly)
        {
            return BreadthFirstWalk.WalkFrom<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, AlgebraStopCheck>(roots, seed, guard);
        }

        return BreadthFirstWalk.WalkFrom<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState, NoStopCheck>(roots, seed, guard);
    }
}
