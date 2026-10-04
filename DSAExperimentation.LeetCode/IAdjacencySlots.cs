namespace DSAExperimentation.LeetCode;

// What LeetCodeAdjacency asks of a slot shape: a fresh slot for node `id`, and how one endpoint of
// an edge records it - its own slot, the far endpoint's id and slot, and the edge's index in the
// caller's list. Instance members on a struct type parameter (§12.4), because a shape may carry the
// edge list a weight is read from; each shape is still its own instantiation, so both members are
// direct calls.
internal interface IAdjacencySlots<TSlot>
{
    TSlot SlotFor(int id);

    void Wire(TSlot slot, int farId, TSlot farSlot, int edgeIndex);
}
