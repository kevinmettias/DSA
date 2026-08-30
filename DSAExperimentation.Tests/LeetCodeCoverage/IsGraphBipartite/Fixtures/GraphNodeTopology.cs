using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite.Fixtures;

internal readonly struct GraphNodeTopology : IGraphTopology<GraphNode, ListChildren<GraphNode>>
{
    public static ListChildren<GraphNode> GetChildren(GraphNode node) => new(node.Neighbors);
}
