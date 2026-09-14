using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

// Bare IGraphTopology, not IDagTopology: the circle is one big cycle, so nothing
// here may promise an engine that there are none.
internal readonly struct CircularArrayTopology : IGraphTopology<CircularArrayNode, ListChildren<CircularArrayNode>>
{
    public static ListChildren<CircularArrayNode> GetChildren(CircularArrayNode node) => new(node.Neighbors);
}
