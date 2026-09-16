namespace DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

// LC 2493's (n, edges) pair list flattened into a plain neighbour array - the
// shape the textbook color-array and distance-array walks want, and the only
// preparation those walks need. Slot i holds node i's neighbours so LeetCode's
// 1..n numbering indexes directly; slot 0 stays empty and every walk starts at
// FirstNode so the placeholder is never mistaken for an isolated component.
//
// It is a type rather than a bare int[][] for the same reason GroupGraph is: so
// a benchmark can hoist construction into [GlobalSetup] and hand the prepared
// input to a second overload that can never be ambiguous with the LeetCode-shaped
// one (ARCHITECTURE.md #17.4). Holding the arrays is all it does - the walks over
// them stay BCL, as a baseline's internals must (#17.5).
internal sealed class GroupAdjacency
{
    public int[][] Neighbors { get; }

    private GroupAdjacency(int[][] neighbors) => Neighbors = neighbors;

    // One empty neighbour list per node id, then both directions of every edge - the
    // layout LeetCodeAdjacency states once for every problem taking an (n, edges) pair.
    public static GroupAdjacency Build(int nodeCount, int[][] edges)
    {
        var neighbors = LeetCodeAdjacency.OneBased<List<int>>(
            nodeCount, edges, _ => [], (list, farId, _, _) => list.Add(farId));

        return new GroupAdjacency(neighbors.Select(list => list.ToArray()).ToArray());
    }
}
