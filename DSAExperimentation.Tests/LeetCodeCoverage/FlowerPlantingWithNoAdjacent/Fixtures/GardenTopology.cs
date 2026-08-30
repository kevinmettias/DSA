using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlowerPlantingWithNoAdjacent.Fixtures;

internal readonly struct GardenTopology : IGraphTopology<GardenNode, ListChildren<GardenNode>>
{
    public static ListChildren<GardenNode> GetChildren(GardenNode node) => new(node.ConnectedGardens);
}
