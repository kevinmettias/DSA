using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestColorValueInADirectedGraph.Fixtures;

internal readonly struct ColorGraphTopology : IGraphTopology<ColorGraphNode, ListChildren<ColorGraphNode>>
{
    public static ListChildren<ColorGraphNode> GetChildren(ColorGraphNode node) => new(node.Successors);
}
