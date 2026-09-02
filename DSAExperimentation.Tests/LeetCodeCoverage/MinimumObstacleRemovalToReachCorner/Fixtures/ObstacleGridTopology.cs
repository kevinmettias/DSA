using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumObstacleRemovalToReachCorner.Fixtures;

internal readonly struct ObstacleGridTopology : IEdgeTopology<ObstacleGridNode, ListEdges<ObstacleGridNode, int>, int>
{
    public static ListEdges<ObstacleGridNode, int> GetEdges(ObstacleGridNode node) => new(node.Edges);
}
