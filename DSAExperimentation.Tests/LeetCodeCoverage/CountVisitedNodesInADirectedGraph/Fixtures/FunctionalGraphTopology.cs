using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountVisitedNodesInADirectedGraph.Fixtures;

internal readonly struct FunctionalGraphTopology : IGraphTopology<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>
{
    public static ListChildren<FunctionalGraphNode> GetChildren(FunctionalGraphNode node) => new(node.Successors);
}
