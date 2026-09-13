using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.CourseScheduleIV;

// The IEdgeTopology witness AllPairsShortestPaths.TryComputeDistances needs in
// order to walk CourseNode - the same one-line shape CityTopology uses for
// LC 1334's weighted node.
internal readonly struct CourseTopology : IEdgeTopology<CourseNode, ListEdges<CourseNode, int>, int>
{
    public static ListEdges<CourseNode, int> GetEdges(CourseNode node) => new(node.Edges);
}
