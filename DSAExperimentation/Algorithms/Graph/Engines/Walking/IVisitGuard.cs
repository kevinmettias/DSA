namespace DSAExperimentation.Algorithms.Graph.Engines.Walking;

// The one axis that actually differs between a tree-only walk and a graph-safe
// walk: whether a child needs to be checked against a visited-set before being
// recursed into or enqueued. Instance-dispatched rather than static-abstract like
// TOrder/TStep - the graph-safe guard carries real state (the HashSet itself), so
// it has to be a value threaded through the walk, not a witness selected purely by
// type.
//
// Internal, like everything else in this chain: IReduceOrderStrategy.Evaluate is
// generic over TGuard, and that interface is internal too, so there's no
// accessibility mismatch to resolve. This does not open up "pick any guard for any
// topology" - DepthFirstWalk/BreadthFirstWalk, the only things that actually
// execute a walk against a topology, stay internal, so nothing outside this
// assembly can reach one no matter which guard it names; and TrackedVisitGuard's
// constructor stays internal too, so outside code cannot even construct a valid
// one. Reduce.Tree/Reduce.Graph remain the only way in, and they still choose the
// guard themselves.
internal interface IVisitGuard<TNode>
    where TNode : class
{
    bool ShouldVisit(TNode node);
}
