namespace DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

// One node per graph vertex. Edges holds both directions every input edge (u, v, w)
// contributes: u -> v at cost w (traversing it as given) and v -> u at cost 2w
// (arriving at v, reversing that same edge, and immediately crossing it back to u,
// exactly LC 3650's per-node switch). A shortest *simple* path never needs a node's
// switch twice, so folding both directions into one adjacency list up front is
// enough - no separate "switch used" bit has to ride along in the search state.
// Answers LC 3650 alone; a fully generic weighted digraph node belongs in
// DataStructures, not here, but nothing that generic exists yet (EdgeGraphNode's own
// doc comment makes the same call for LC 3123's undirected, same-weight-both-ways
// shape).
internal sealed class ReversalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, ReversalGraphNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
