using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Engines.Reducing;

// The traversal-order axis of a reduce, injected as a strategy the same way
// IFoldEvaluationStrategy is for fold - but unlike that one, this is a genuinely
// semantic choice, not a swappable evaluation detail: picking depth-first vs
// breadth-first here can change the result whenever an algebra's Enter/Exit aren't
// order-insensitive (contrast IFoldAlgebra's Combine, which only ever sees a node
// together with its already-folded children, never visit order or timing).
//
// Generic over TGuard rather than fixed to one topology tier, so the same strategy
// (DepthFirstReduceOrder, BreadthFirstReduceOrder) serves both Reduce.Tree and
// Reduce.Graph - the topology-tier split (which guard gets constructed, and which
// topology bound proves it's safe to skip tracking) lives entirely at that
// entry-point layer, not duplicated into a second strategy interface per tier the
// way IGraphReduceTraversalStrategy used to.
internal interface IReduceOrderStrategy<TNode>
    where TNode : class
{
    static abstract TState Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TAlgebra, TState>(
        TNode root, TGuard guard)
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : IVisitGuard<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>;
}
