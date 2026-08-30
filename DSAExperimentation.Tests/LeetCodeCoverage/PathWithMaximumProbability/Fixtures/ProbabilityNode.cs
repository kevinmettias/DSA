namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMaximumProbability.Fixtures;

// One node per labeled vertex. Edge weight is -log(probability), not the
// probability itself - see PathWithMaximumProbabilityTests for why that
// turns "maximize a product" into ShortestPath.Dijkstra's own "minimize a
// non-negative sum."
internal sealed class ProbabilityNode(int id)
{
    public int Id { get; } = id;

    public List<(double Cost, ProbabilityNode Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
