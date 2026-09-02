using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra
// needs to run over EdgeGraphNode - the same one-line shape BranchTopology
// already uses for NumberOfPossibleSetsOfClosingBranches' own weighted node.
internal readonly struct EdgeGraphTopology : IEdgeTopology<EdgeGraphNode, ListEdges<EdgeGraphNode, long>, long>
{
    public static ListEdges<EdgeGraphNode, long> GetEdges(EdgeGraphNode node) => new(node.Edges);
}
