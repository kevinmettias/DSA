namespace DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

// One node per intersection. Edges double as both ShortestPath.Dijkstra's weighted
// edge list (WaysEdgeTopology) and, once Dist is populated from that same Dijkstra
// run (computed from the destination, so Dist is each node's own shortest travel
// time to it), WaysChildTopology's shortest-path-only child list - no second
// adjacency copy needed.
//
// Weights are long because LC 1976 allows up to 200 roads of 1e9 time each, which
// overflows int well before the last intersection is reached.
internal sealed class WaysNode(int id)
{
    public int Id { get; } = id;

    public List<(long Weight, WaysNode Target)> Edges { get; } = [];

    public long Dist { get; set; }

    public override string ToString() => Id.ToString();
}
