using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

// Bare IGraphTopology, not IDagTopology: every walk in LC 2876 ends in a cycle by
// construction, so nothing here may promise an engine that there are none.
internal readonly struct FunctionalGraphTopology : IGraphTopology<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>
{
    public static ListChildren<FunctionalGraphNode> GetChildren(FunctionalGraphNode node) => new(node.Successors);
}
