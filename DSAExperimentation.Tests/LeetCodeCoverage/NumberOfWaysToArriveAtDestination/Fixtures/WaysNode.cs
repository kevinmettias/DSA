namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination.Fixtures;

// One node per intersection. Edges double as both ShortestPath.Dijkstra's weighted
// edge list (WaysEdgeTopology) and, once Dist is populated from that same Dijkstra
// run (computed from the destination, so Dist is each node's own shortest distance
// to it), WaysChildTopology's shortest-path-only child list - no second adjacency
// copy needed.
internal sealed class WaysNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, WaysNode Target)> Edges { get; } = [];

    public long Dist { get; set; }

    public override string ToString() => Id.ToString();
}
