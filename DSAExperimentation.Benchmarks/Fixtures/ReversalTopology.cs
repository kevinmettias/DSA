using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal readonly struct ReversalTopology : IGraphTopology<PositionNode, ReversalChildren>
{
    public static ReversalChildren GetChildren(PositionNode node) => new(node);
}
