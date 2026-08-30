using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RaceCar.Fixtures;

internal readonly struct RaceCarTopology : IGraphTopology<RaceCarNode, ListChildren<RaceCarNode>>
{
    public static ListChildren<RaceCarNode> GetChildren(RaceCarNode node) => new(node.Neighbors);
}
