namespace DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

// LC 1042's (n, paths) input materialized once as GardenNodes wired both ways,
// since a path is bidirectional.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared graph to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class GardenNetwork
{
    // Gardens in LeetCode's own 1..n order, so slot i holds garden i + 1 and the
    // answer array's layout falls out of the walk order.
    public IReadOnlyList<GardenNode> Gardens { get; }

    private GardenNetwork(IReadOnlyList<GardenNode> gardens) => Gardens = gardens;

    public static GardenNetwork Build(int n, int[][] paths)
    {
        // Slot id holds garden id, so LeetCode's 1..n numbering indexes directly;
        // slot 0 is a placeholder that carries no edges and is not returned.
        var byId = new GardenNode[n + 1];

        for (var id = GardenNumbering.FirstGarden; id <= n; id++)
        {
            byId[id] = new GardenNode(id);
        }

        foreach (var path in paths)
        {
            byId[path[0]].ConnectedGardens.Add(byId[path[1]]);
            byId[path[1]].ConnectedGardens.Add(byId[path[0]]);
        }

        return new GardenNetwork(byId[GardenNumbering.FirstGarden..]);
    }
}
