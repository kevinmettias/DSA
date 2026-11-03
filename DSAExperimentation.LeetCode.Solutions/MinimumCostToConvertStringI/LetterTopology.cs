using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.Solutions.MinimumCostToConvertStringI;

// The IEdgeTopology witness Algorithms.ShortestPaths.AllPairsShortestPaths
// needs to run Floyd-Warshall over LetterNode - the same one-line shape
// BranchTopology already uses for a different weighted node.
internal readonly struct LetterTopology : IEdgeTopology<LetterNode, ListEdges<LetterNode, int>, int>
{
    public static ListEdges<LetterNode, int> GetEdges(LetterNode node) => new(node.Edges);
}
