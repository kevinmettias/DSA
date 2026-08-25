namespace DSAExperimentation.Algorithms.Reducing;

// Proof that a runtime-parameterized query doesn't always need a bespoke facade
// (contrast LowestCommonAncestor): "distance to some specific target" only needs to
// know the target to answer the question, not to compute the answer - Enter can
// record every reachable node's distance with zero knowledge of which one the
// caller actually wants, purely structurally, and the runtime lookup happens after
// the reduce completes, in ordinary code. The one thing this gives up versus a
// bespoke early-terminating BFS is efficiency: it computes distances to every
// reachable node, not just the target, in exchange for staying a plain algebra
// reusable by Reduce.Graph as-is.
//
// Topology-agnostic, not grid-specific - works for any TNode reachable from any
// IGraphTopology-compatible root.
internal readonly struct DistanceMapReduceAlgebra<TNode> : IReduceAlgebra<TNode, Dictionary<TNode, int>>
    where TNode : notnull
{
    public static Dictionary<TNode, int> Seed => [];

    public static Dictionary<TNode, int> Enter(Dictionary<TNode, int> state, TNode node, int depth)
    {
        state[node] = depth;
        return state;
    }
}
