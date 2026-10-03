namespace DSAExperimentation.Algorithms.Walking;

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
//
// A readonly struct whose one field is the shared set, and every engine constrains
// TGuard to struct. As a class, the guard made every graph walk run as code shared
// across reference types, with ShouldVisit an interface call per child; as a struct,
// each walk is JIT-specialized for this guard and ShouldVisit inlines, the same zero
// cost UnguardedVisit gets on the tree tier, and a walk no longer allocates the guard.
// Copies alias the one HashSet by design - the way ListChildren's copies alias its
// List (11.2) - so threading the guard by value still shares the set, which is what
// stops a walk looping. default(TrackedVisitGuard<TNode>) holds no set and throws on
// first use; only Reduce and the traversal entry points construct one.
internal readonly struct TrackedVisitGuard<TNode> : IVisitGuard<TNode>
    where TNode : class
{
    private readonly HashSet<TNode> _visited;

    // Internal: a validly constructed guard is only useful paired with the internal
    // engine, so construction stays out of outside hands.
    internal TrackedVisitGuard(HashSet<TNode> visited) => _visited = visited;

    public bool ShouldVisit(TNode node) => _visited.Add(node);
}
