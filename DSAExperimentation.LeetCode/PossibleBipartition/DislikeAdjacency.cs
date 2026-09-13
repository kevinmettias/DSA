namespace DSAExperimentation.LeetCode.PossibleBipartition;

// LC 886's (n, dislikes) pair list flattened into a plain neighbour array - the
// shape the textbook color-array walk wants, and the only preparation that walk
// needs. Slot i holds person i's neighbours so LeetCode's 1..n numbering indexes
// directly; slot 0 stays empty.
//
// It is a type rather than a bare int[][] for the same reason DislikeGraph is:
// so a benchmark can hoist construction into [GlobalSetup] and hand the prepared
// input to a second overload that can never be ambiguous with the LeetCode-shaped
// one (ARCHITECTURE.md #17.4). Holding the arrays is all it does - the walk over
// them stays BCL, as a baseline's internals must (#17.5).
internal sealed class DislikeAdjacency
{
    private DislikeAdjacency(int[][] neighbors) => Neighbors = neighbors;

    public int[][] Neighbors { get; }

    public static DislikeAdjacency Build(int n, int[][] dislikes)
    {
        var neighbors = new List<int>[n + 1];

        for (var id = 0; id <= n; id++)
        {
            neighbors[id] = [];
        }

        foreach (var pair in dislikes)
        {
            neighbors[pair[0]].Add(pair[1]);
            neighbors[pair[1]].Add(pair[0]);
        }

        return new DislikeAdjacency(neighbors.Select(list => list.ToArray()).ToArray());
    }
}
