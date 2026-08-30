using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CriticalConnectionsInANetwork.Fixtures;

internal readonly struct ServerTopology : IGraphTopology<ServerNode, ListChildren<ServerNode>>
{
    public static ListChildren<ServerNode> GetChildren(ServerNode node) => new(node.Neighbors);
}
