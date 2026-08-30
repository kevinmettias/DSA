using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LoudAndRich.Fixtures;

internal readonly struct PersonTopology : IGraphTopology<PersonNode, ListChildren<PersonNode>>
{
    public static ListChildren<PersonNode> GetChildren(PersonNode node) => new(node.Poorer);
}
