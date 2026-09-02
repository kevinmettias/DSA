using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

// The IEdgeTopology witness Algorithms.ShortestPaths.AllPairsShortestPaths needs
// to run Floyd-Warshall over BranchNode - the same one-line shape
// WeightedGridTopology already uses for a different weighted node.
internal readonly struct BranchTopology : IEdgeTopology<BranchNode, ListEdges<BranchNode, int>, int>
{
    public static ListEdges<BranchNode, int> GetEdges(BranchNode node) => new(node.Edges);
}
