using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// The IEdgeTopology witness Algorithms.ShortestPaths.AllPairsShortestPaths
// needs to run Floyd-Warshall over StringNode - the same one-line shape
// LetterTopology already uses for a different weighted node.
internal readonly struct StringTopology : IEdgeTopology<StringNode, ListEdges<StringNode, int>, int>
{
    public static ListEdges<StringNode, int> GetEdges(StringNode node) => new(node.Edges);
}
