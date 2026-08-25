using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Graph.Engines.Walking;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Engines.Reducing;

internal readonly struct BreadthFirstReduceOrder<TNode> : IReduceOrderStrategy<TNode>
    where TNode : class
{
    public static TState Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TGuard guard)
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : IVisitGuard<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
            root, TAlgebra.Seed, guard);
}
