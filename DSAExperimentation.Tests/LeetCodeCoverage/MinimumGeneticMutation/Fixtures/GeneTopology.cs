using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation.Fixtures;

internal readonly struct GeneTopology : IGraphTopology<GeneNode, ListChildren<GeneNode>>
{
    public static ListChildren<GeneNode> GetChildren(GeneNode node) => new(node.Neighbors);
}
