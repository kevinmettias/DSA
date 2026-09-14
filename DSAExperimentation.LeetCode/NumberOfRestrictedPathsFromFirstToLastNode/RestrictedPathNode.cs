namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// One node per graph vertex. Edges double as both ShortestPath.Dijkstra's weighted
// edge list (RestrictedPathEdgeTopology) and, once Dist is populated from that same
// Dijkstra run, DagFold's restricted-path child list (RestrictedPathChildTopology
// keeps only neighbors whose Dist is strictly smaller) - no second adjacency copy
// needed.
//
// Dist is what makes this LC 1786's own node rather than a generic weighted one: it
// is the distance to the *last* node specifically, the quantity the problem's
// "restricted" rule is defined against (EdgeGraphNode makes the same call for LC
// 3123's plainer shape).
internal sealed class RestrictedPathNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, RestrictedPathNode Target)> Edges { get; } = [];

    public int Dist { get; set; }

    public override string ToString() => Id.ToString();
}
