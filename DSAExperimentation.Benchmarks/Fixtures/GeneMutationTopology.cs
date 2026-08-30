using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal readonly struct GeneMutationTopology : IGraphTopology<GeneMutationNode, ListChildren<GeneMutationNode>>
{
    public static ListChildren<GeneMutationNode> GetChildren(GeneMutationNode node) => new(node.Neighbors);
}
