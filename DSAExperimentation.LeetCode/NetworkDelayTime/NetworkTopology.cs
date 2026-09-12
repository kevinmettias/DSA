using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NetworkDelayTime;

internal readonly struct NetworkTopology : IEdgeTopology<NetworkNode, ListEdges<NetworkNode, int>, int>
{
    public static ListEdges<NetworkNode, int> GetEdges(NetworkNode node) => new(node.Edges);
}
