namespace DSAExperimentation.Graph;

// The one axis that actually differs between a tree-only walk and a graph-safe
// walk: whether a child needs to be checked against a visited-set before being
// recursed into or enqueued. Instance-dispatched rather than static-abstract like
// TOrder/TStep - the graph-safe guard carries real state (the HashSet itself), so
// it has to be a value threaded through the walk, not a witness selected purely by
// type.
//
// Public, not internal: IReduceOrderStrategy.Evaluate is generic over TGuard, so
// this has to be at least as visible as that public interface member for the
// constraint to be legal. This does not reopen "pick any guard for any topology" -
// DepthFirstWalk/BreadthFirstWalk, the only things that actually execute a walk
// against a topology, stay internal, so nothing outside this assembly can reach one
// no matter which guard it names; and TrackedVisitGuard's constructor (below) stays
// internal too, so outside code cannot even construct a valid one. Reduce.Tree/
// Reduce.Graph remain the only way in, and they still choose the guard themselves.
public interface IVisitGuard<TNode>
    where TNode : class
{
    bool ShouldVisit(TNode node);
}

// The tree-only guard: ITreeTopology already promises unique ancestry, so every
// child is visited unconditionally. Stateless, so the constrained call through this
// should cost nothing beyond what a hardcoded "if (true)" would.
public readonly struct UnguardedVisit<TNode> : IVisitGuard<TNode>
    where TNode : class
{
    public bool ShouldVisit(TNode node) => true;
}

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
public readonly struct TrackedVisitGuard<TNode> : IVisitGuard<TNode>
    where TNode : class
{
    private readonly HashSet<TNode> _visited;

    // Internal even though the struct itself is public: construction is the one
    // remaining thing this type needs to keep out of outside hands, since a validly
    // constructed guard is only useful paired with the internal engine anyway.
    internal TrackedVisitGuard(HashSet<TNode> visited) => _visited = visited;

    public bool ShouldVisit(TNode node) => _visited.Add(node);
}
