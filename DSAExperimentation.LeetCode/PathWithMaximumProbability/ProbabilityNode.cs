namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// One node per labeled vertex. Edge weight is -log(probability), not the probability
// itself - see PathWithMaximumProbabilitySolution for why that turns "maximize a
// product" into ShortestPath.Dijkstra's own "minimize a non-negative sum" - in the
// same "weight, target" edge-list shape IEdgeTopology needs that RecoveryNode and
// WeightedGridNode already establish.
internal sealed class ProbabilityNode(int id)
{
    public int Id { get; } = id;

    public List<(double Cost, ProbabilityNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
