using System.Runtime.CompilerServices;

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
//
// The difference arrives in one of two forms. A slot shape many problems share - a bare
// neighbour id, a neighbour beside the edge's weight - is a struct witness
// (IAdjacencySlots), so filling a slot is a direct call; those are the shapes the
// baselines build inside their timed region. A shape one problem alone uses, usually a
// composed arm's node graph built once in setup, arrives as two lambdas instead, which
// costs a delegate call per slot and per edge endpoint but no type of its own.
internal static class LeetCodeAdjacency
{
    // Nodes numbered 0..n-1: slots[i] is node i.
    //
    // Never inlined. A layout runs once per solve, so inlining it saves nothing, and a
    // baseline that calls it inline loses the JIT inlining its own hot loop would have
    // had. Measured over identical lists built in an identical order:
    // CountValidPathsInATree's per-pair walk ran 45% slower and
    // MinimumCostWalkInWeightedGraph's 55% slower without this, and with it both come
    // within the benchmark's noise (about 10%) of their old hand-written loops.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TSlot[] ZeroBased<TSlot, TSlots>(int nodeCount, int[][] edges, TSlots slots)
        where TSlots : struct, IAdjacencySlots<TSlot> =>
        Build<TSlot, TSlots>(nodeCount, edges, slots);

    // Nodes numbered 1..n: slots[i] is node i, and slots[0] is the placeholder that
    // numbering leaves behind - built like any other slot, and simply never named by an
    // edge. Never inlined, for the reason ZeroBased gives.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TSlot[] OneBased<TSlot, TSlots>(int nodeCount, int[][] edges, TSlots slots)
        where TSlots : struct, IAdjacencySlots<TSlot> =>
        Build<TSlot, TSlots>(nodeCount + 1, edges, slots);

    public static TSlot[] ZeroBased<TSlot>(
        int nodeCount, int[][] edges, Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire) =>
        Build<TSlot, CallbackSlots<TSlot>>(nodeCount, edges, new CallbackSlots<TSlot>(slotFor, wire));

    public static TSlot[] OneBased<TSlot>(
        int nodeCount, int[][] edges, Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire) =>
        Build<TSlot, CallbackSlots<TSlot>>(nodeCount + 1, edges, new CallbackSlots<TSlot>(slotFor, wire));

    // The layout itself: `slotCount` slots indexed by id, each built by the slot rule,
    // then every edge recorded at both of the endpoints it connects. The public forms
    // differ only in the slot count they hand this, which is the whole of what a
    // numbering convention changes, and in how the slot rule arrives.
    private static TSlot[] Build<TSlot, TSlots>(int slotCount, int[][] edges, TSlots rule)
        where TSlots : struct, IAdjacencySlots<TSlot>
    {
        var slots = new TSlot[slotCount];

        for (var id = 0; id < slotCount; id++)
        {
            slots[id] = rule.SlotFor(id);
        }

        for (var edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
        {
            var edge = edges[edgeIndex];
            rule.Wire(slots[edge[0]], edge[1], slots[edge[1]], edgeIndex);
            rule.Wire(slots[edge[1]], edge[0], slots[edge[0]], edgeIndex);
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

    // The two lambdas, carried as the slot rule Build takes.
    private readonly struct CallbackSlots<TSlot>(Func<int, TSlot> slotFor, EdgeWiring<TSlot> wire)
        : IAdjacencySlots<TSlot>
    {
        public TSlot SlotFor(int id) => slotFor(id);

        public void Wire(TSlot slot, int farId, TSlot farSlot, int edgeIndex) => wire(slot, farId, farSlot, edgeIndex);
    }
}
