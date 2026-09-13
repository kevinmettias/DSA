using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.JumpGameIV;

internal readonly struct ValueHopTopology : IEdgeTopology<ValueHopNode, ListEdges<ValueHopNode, int>, int>
{
    public static ListEdges<ValueHopNode, int> GetEdges(ValueHopNode node) => new(node.Edges);
}
