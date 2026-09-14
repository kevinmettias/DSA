using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.DesignGraphWithShortestPathCalculator;

internal readonly struct ShortestPathCalculatorTopology
    : IEdgeTopology<ShortestPathCalculatorNode, ListEdges<ShortestPathCalculatorNode, int>, int>
{
    public static ListEdges<ShortestPathCalculatorNode, int> GetEdges(ShortestPathCalculatorNode node)
        => new(node.Edges);
}
