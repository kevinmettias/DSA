namespace DSAExperimentation.Graph.Engines.Walking;

// The graph-safe guard: IGraphTopology promises nothing about cycles or unique
// ancestry, so each child is checked against (and added to) a visited-set before
// being walked. This HashSet is the one real, unavoidable allocation the graph path
// pays that the tree path never needs to. A revisited node is simply skipped, cycle
// or not - unlike CheckedFold, a reduce has no well-definedness problem a cycle could
// break (there's no Combine that needs to see a value which doesn't exist yet), so
// silent truncation is a correct, sufficient answer, not a shortcut.
//
// Deliberately the only graph-safe guard - unlike Fold, there's no useful third tier
// here. DagFold earns its keep over CheckedFold by dropping a *second*, separate
// cost (the inProgress recursion-stack check) once acyclicity is promised, while
// still paying the completed/memoization cost every DAG needs. This guard has no
// such split: one visited-set already does double duty as both "don't reprocess a
// shared descendant" (unavoidable for any DAG, trusted or not) and "don't loop
// forever" (unavoidable for any graph) - there's no second cost a trusted promise
// could let it shed, so a "trusted-DAG reduce" would compile to this exact guard
// under a narrower bound. And matching CheckedFold's throw-on-a-real-cycle fidelity
// would need the guard to know recursion-stack membership, not just "seen" - which
// means the walk engine notifying it when a child's subtree finishes, a cost every
// guard (including the tree-only one) would pay on every call, for a distinction
// only meaningful to a DFS-shaped walk. Not worth taxing the zero-cost path for.
// A reference type, not a value type: this guard's entire purpose is one shared,
// mutating HashSet threaded by identity through the whole walk. Wrapping that in a
// struct would make every copy look like an independent value while secretly
// aliasing the same set - the sharing is the point, so the type says so instead of
// hiding it behind copy syntax.
internal sealed class TrackedVisitGuard<TNode> : IVisitGuard<TNode>
    where TNode : class
{
    private readonly HashSet<TNode> _visited;

    // Internal: construction is the one remaining thing this type needs to keep out
    // of outside hands, since a validly constructed guard is only useful paired with
    // the internal engine anyway.
    internal TrackedVisitGuard(HashSet<TNode> visited) => _visited = visited;

    public bool ShouldVisit(TNode node) => _visited.Add(node);
}
