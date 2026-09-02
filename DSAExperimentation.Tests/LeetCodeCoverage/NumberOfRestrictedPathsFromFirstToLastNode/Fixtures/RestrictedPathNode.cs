namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode.Fixtures;

// One node per graph vertex. Edges double as both ShortestPath.Dijkstra's weighted
// edge list (RestrictedPathEdgeTopology) and, once Dist is populated from that same
// Dijkstra run, DagFold's restricted-path child list (RestrictedPathChildTopology
// keeps only neighbors whose Dist is strictly smaller) - no second adjacency copy
// needed.
internal sealed class RestrictedPathNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, RestrictedPathNode Target)> Edges { get; } = [];

    public int Dist { get; set; }

    public override string ToString() => Id.ToString();
}
