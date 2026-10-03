namespace DSAExperimentation.LeetCode.Tests;

// Every undirected-graph problem builds its adjacency through these two entry points, so a
// slot built at the wrong index or an edge recorded at one endpoint only would surface as a
// wrong answer in some unrelated problem. Each slot here records what the wiring told it -
// the far endpoint and the edge's index - so the layout itself is what is asserted.
public sealed partial class LeetCodeAdjacencyTests
{
    [Fact]
    public void ZeroBased_TriangleEdges_RecordsEachEdgeAtBothEndpoints()
    {
        var slots = LeetCodeAdjacency.ZeroBased(3, [[0, 1], [1, 2], [2, 0]], _ => new List<string>(), Wire);

        Assert.Equal(["1@0", "2@2"], slots[0]);
        Assert.Equal(["0@0", "2@1"], slots[1]);
        Assert.Equal(["1@1", "0@2"], slots[2]);
    }

    // Numbered from one, the graph's nodes take slots 1..n and slot zero is built but never
    // named by an edge.
    [Fact]
    public void OneBased_EdgesNumberedFromOne_LeavesSlotZeroUnwired()
    {
        var slots = LeetCodeAdjacency.OneBased(2, [[1, 2]], _ => new List<string>(), Wire);

        Assert.Equal(3, slots.Length);
        Assert.Empty(slots[0]);
        Assert.Equal(["2@0"], slots[1]);
        Assert.Equal(["1@0"], slots[2]);
    }

    private static void Wire(List<string> slot, int farId, List<string> farSlot, int edgeIndex)
        => slot.Add($"{farId}@{edgeIndex}");
}
