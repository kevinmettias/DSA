using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

// Computed on demand from the node's own reversed adjacency, the same way
// GridChildren filters its (at most 4) directions to the ones the node's Grid
// currently allows - here the filter is "weight <= MaxWeight" instead of
// "in bounds and passable", and the candidate list can be far longer than 4, so
// the qualifying neighbors are captured once per GetChildren call instead of
// rescanned on every Get.
internal readonly struct EdgeWeightChildren : IChildren<EdgeWeightNode>
{
    private readonly EdgeWeightNode _node;
    private readonly ReadOnlyMemory<(int To, int Weight)> _reachable;

    public int Count => _reachable.Length;

    public EdgeWeightChildren(EdgeWeightNode node)
    {
        _node = node;
        _reachable = node.Graph.NeighborsOf(node.Id).Where(edge => edge.Weight <= node.MaxWeight).ToArray();
    }

    public EdgeWeightNode Get(int index) => new(_reachable.Span[index].To, _node.Graph, _node.MaxWeight);
}
