using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.LongestCycleInAGraph;

// Bare IGraphTopology, not IDagTopology: finding the cycles is the whole problem,
// so nothing here may promise an engine that there are none.
internal readonly struct FunctionalGraphTopology : IGraphTopology<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>
{
    public static ListChildren<FunctionalGraphNode> GetChildren(FunctionalGraphNode node) => new(node.Successors);
}
