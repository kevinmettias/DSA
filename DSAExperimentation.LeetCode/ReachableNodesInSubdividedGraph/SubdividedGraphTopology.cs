using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over SubdividedGraphNode - the same one-line shape EdgeGraphTopology already
// uses for FindEdgesInShortestPaths' own weighted node.
internal readonly struct SubdividedGraphTopology
    : IEdgeTopology<SubdividedGraphNode, ListEdges<SubdividedGraphNode, int>, int>
{
    public static ListEdges<SubdividedGraphNode, int> GetEdges(SubdividedGraphNode node) => new(node.Edges);
}
