using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal readonly struct RemainderTopology : IGraphTopology<RemainderNode, ListChildren<RemainderNode>>
{
    public static ListChildren<RemainderNode> GetChildren(RemainderNode node) => new(node.Neighbors);
}
