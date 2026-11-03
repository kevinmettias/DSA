using System.Numerics;

namespace DSAExperimentation.LeetCode;

// Each node's neighbours beside the weight LeetCode puts in an edge's third column, in edge order,
// read as TWeight - int as given, or long where a path's summed weight can pass int's range. TWeight
// is a value type at every use, so each is its own instantiation and the conversion inlines away.
internal readonly struct WeightedNeighborSlots<TWeight>(int[][] edges) : IAdjacencySlots<List<(int To, TWeight Weight)>>
    where TWeight : INumberBase<TWeight>
{
    private const int WeightColumn = 2;

    public List<(int To, TWeight Weight)> SlotFor(int id) => [];

    public void Wire(List<(int To, TWeight Weight)> slot, int farId, List<(int To, TWeight Weight)> farSlot, int edgeIndex)
        => slot.Add((farId, TWeight.CreateTruncating(edges[edgeIndex][WeightColumn])));
}
