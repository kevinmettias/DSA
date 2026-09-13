using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

internal readonly struct PointTopology : IEdgeTopology<PointNode, ListEdges<PointNode, int>, int>
{
    public static ListEdges<PointNode, int> GetEdges(PointNode node) => new(node.Edges);
}
