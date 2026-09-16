namespace DSAExperimentation.LeetCode;

// LeetCode states an undirected graph as a node count plus a list of edges, and every
// problem taking that pair does the same two things with it: give each node a slot,
// then record each edge in both of the slots it connects. What a slot holds is the
// problem's own business - a bare neighbour id, a neighbour beside the road's weight,
// a neighbouring node object, the index the edge came from - so the layout is written
// once here and that one difference arrives as a callback.
//
// Slots are indexed by node id, which is what lets an edge's own values address them
// directly. LeetCode numbers such a graph's nodes either 0..n-1 or 1..n, so a graph
// numbered from one carries a slot zero that no edge ever names.
//
// Like LeetCodeAnswer, this is fixed by the problem set rather than by any algorithm,
// which is why it lives at the root of the LeetCode tier rather than in the folder of
// whichever problem happened to need it first.
internal static class LeetCodeAdjacency
{
    // Nodes numbered 0..n-1: slots[i] is node i.
    public static TSlot[] ZeroBased<TSlot>(
        int nodeCount, int[][] edges, Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire) =>
        Build(nodeCount, edges, slotFor, wire);

    // Nodes numbered 1..n: slots[i] is node i, and slots[0] is the placeholder that
    // numbering leaves behind - built by `slotFor` like any other slot, and simply
    // never named by an edge.
    public static TSlot[] OneBased<TSlot>(
        int nodeCount, int[][] edges, Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire) =>
        Build(nodeCount + 1, edges, slotFor, wire);

    // The layout itself: `slotCount` slots indexed by id, each built by `slotFor`, then
    // every edge recorded at both of the endpoints it connects. The two public forms
    // differ only in the slot count they hand this, which is the whole of what a
    // numbering convention changes.
    private static TSlot[] Build<TSlot>(
        int slotCount, int[][] edges, Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire)
    {
        var slots = new TSlot[slotCount];

        for (var id = 0; id < slotCount; id++)
        {
            slots[id] = slotFor(id);
        }

        for (var edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
        {
            var edge = edges[edgeIndex];
            wire(slots[edge[0]], edge[1], slots[edge[1]], edgeIndex);
            wire(slots[edge[1]], edge[0], slots[edge[0]], edgeIndex);
        }

        return slots;
    }

    // What one endpoint of an edge is told about it: its own slot, the far endpoint's
    // id, the far endpoint's slot, and the edge's index in `edges`. Both endpoints
    // travel together because a slot holding a node object needs the node while one
    // holding a plain id needs the number; the index is there because the edge list
    // the caller already has in hand is the only place a weight or an edge number can
    // be read from.
    internal delegate void EdgeWiring<TSlot>(TSlot slot, int farId, TSlot farSlot, int edgeIndex);
}
