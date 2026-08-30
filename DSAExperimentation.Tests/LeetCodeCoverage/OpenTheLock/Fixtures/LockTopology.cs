using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OpenTheLock.Fixtures;

internal readonly struct LockTopology : IGraphTopology<LockNode, ListChildren<LockNode>>
{
    public static ListChildren<LockNode> GetChildren(LockNode node) => new(node.Neighbors);
}
