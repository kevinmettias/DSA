namespace DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

// LC 3600's own edge encoding, split once into the two edge kinds every strategy
// needs to treat differently: must-edges are forced into the spanning tree and can
// never be upgraded, optional edges may be skipped, used as-is, or doubled in
// strength (at most once, at a cost of one of the k upgrades). Parsing
// edges[i] = [u, v, s, must] into these two lists is exactly the kind of
// per-problem input shaping the hoisted-overload rule (ARCHITECTURE.md 17.4)
// charges to [GlobalSetup] instead of the search itself.
internal sealed class StabilityGraph
{
    private StabilityGraph(
        int nodeCount, List<(int U, int V, int Strength)> mustEdges, List<(int U, int V, int Strength)> optionalEdges)
    {
        NodeCount = nodeCount;
        MustEdges = mustEdges;
        OptionalEdges = optionalEdges;
    }

    public int NodeCount { get; }

    public List<(int U, int V, int Strength)> MustEdges { get; }

    public List<(int U, int V, int Strength)> OptionalEdges { get; }

    public static StabilityGraph Build(int n, int[][] edges)
    {
        var mustEdges = new List<(int U, int V, int Strength)>();
        var optionalEdges = new List<(int U, int V, int Strength)>();

        foreach (var edge in edges)
        {
            var (u, v, strength, must) = (edge[0], edge[1], edge[2], edge[3]);
            var bucket = must == 1 ? mustEdges : optionalEdges;
            bucket.Add((u, v, strength));
        }

        return new StabilityGraph(n, mustEdges, optionalEdges);
    }
}
