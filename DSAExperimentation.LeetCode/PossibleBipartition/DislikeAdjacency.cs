namespace DSAExperimentation.LeetCode.PossibleBipartition;

// LC 886's (personCount, dislikes) pair list flattened into a plain neighbour
// array - the shape the textbook color-array walk wants, and the only preparation
// that walk needs. Slot i holds person i's neighbours so LeetCode's 1..personCount
// numbering indexes directly; slot 0 stays empty.
//
// It is a type rather than a bare int[][] for the same reason DislikeGraph is:
// so a benchmark can hoist construction into [GlobalSetup] and hand the prepared
// input to a second overload that can never be ambiguous with the LeetCode-shaped
// one (ARCHITECTURE.md #17.4). Holding the arrays is all it does - the walk over
// them stays BCL, as a baseline's internals must (#17.5).
internal sealed class DislikeAdjacency
{
    public int[][] Neighbors { get; }

    private DislikeAdjacency(int[][] neighbors) => Neighbors = neighbors;

    // One empty neighbour list per person id, then both directions of every dislike
    // pair - the layout LeetCodeAdjacency states once for every problem taking an
    // (n, edges) pair.
    public static DislikeAdjacency Build(int personCount, int[][] dislikes)
    {
        var neighbors = LeetCodeAdjacency.OneBased<List<int>>(
            personCount, dislikes, _ => [], (list, farId, _, _) => list.Add(farId));

        return new DislikeAdjacency(neighbors.Select(list => list.ToArray()).ToArray());
    }
}
