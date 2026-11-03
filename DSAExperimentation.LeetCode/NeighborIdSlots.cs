namespace DSAExperimentation.LeetCode;

// Each node's neighbour ids, in edge order - the adjacency list a textbook graph walk takes.
internal readonly struct NeighborIdSlots : IAdjacencySlots<List<int>>
{
    public List<int> SlotFor(int id) => [];

    public void Wire(List<int> slot, int farId, List<int> farSlot, int edgeIndex) => slot.Add(farId);
}
