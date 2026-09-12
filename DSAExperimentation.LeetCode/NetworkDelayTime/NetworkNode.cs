namespace DSAExperimentation.LeetCode.NetworkDelayTime;

// LC 743's own directed, non-negative-weight edge list, kept local to this problem
// folder rather than in DataStructures/: no other migrated problem shares this exact
// shape yet, and Tests' own WeightedNode / Benchmarks' own WeightedGraphNode already
// cover the general "weighted graph node" fixture role for a dozen still-unmigrated
// problems - duplicating a third copy here would recreate the exact defect this
// migration exists to remove, not fix it.
internal sealed class NetworkNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, NetworkNode Target)> Edges { get; } = [];
}
