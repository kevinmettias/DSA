namespace DSAExperimentation.LeetCode.FlowerPlantingWithNoAdjacent;

// LC 1042's (gardenCount, paths) input materialized once as GardenNodes wired both
// ways, since a path is bidirectional.
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

    // A GardenNode per garden id, then both directions of every path.
    // LeetCodeAdjacency states that layout once for every problem taking an (n, edges)
    // pair; slot id holding garden id is this problem being numbered 1..n, and the
    // placeholder that numbering leaves at slot 0 is sliced off below.
    public static GardenNetwork Build(int gardenCount, int[][] paths)
    {
        var byId = LeetCodeAdjacency.OneBased<GardenNode>(
            gardenCount, paths, id => new GardenNode(id), (garden, _, farGarden, _) => garden.ConnectedGardens.Add(farGarden));

        return new GardenNetwork(byId[GardenNumbering.FirstGarden..]);
    }
}
