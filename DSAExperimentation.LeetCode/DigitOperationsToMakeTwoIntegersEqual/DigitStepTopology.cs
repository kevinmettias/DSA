using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra
// needs to run over DigitStepNode - the same one-line shape EdgeGraphTopology
// already uses for FindEdgesInShortestPaths' own weighted node.
internal readonly struct DigitStepTopology
    : IEdgeTopology<DigitStepNode, ListEdges<DigitStepNode, int>, int>
{
    public static ListEdges<DigitStepNode, int> GetEdges(DigitStepNode node) => new(node.Edges);
}
