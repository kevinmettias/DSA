namespace DSAExperimentation.LeetCode.NetworkRecoveryPathways;

// One node per graph vertex; Edges is rebuilt in place for each binary-search
// threshold RecoveryNetwork.Rebuild tries - the same "weight, target" edge-list
// shape IEdgeTopology needs that WeightedGridNode already establishes.
internal sealed class RecoveryNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, RecoveryNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
