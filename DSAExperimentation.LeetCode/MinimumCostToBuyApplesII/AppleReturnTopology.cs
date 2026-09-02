using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

// The IEdgeTopology witness for the return (apple-laden) leg of a trip - the same
// edges as AppleForwardTopology's, but weighted by cost * tax rather than cost
// alone, since carrying apples pays the tax multiplier on every road used.
internal readonly struct AppleReturnTopology : IEdgeTopology<AppleNode, ListEdges<AppleNode, long>, long>
{
    public static ListEdges<AppleNode, long> GetEdges(AppleNode node) => new(node.ReturnEdges);
}
