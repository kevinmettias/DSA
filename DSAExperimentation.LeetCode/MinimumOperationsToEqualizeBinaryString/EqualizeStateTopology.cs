using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

// A general graph, not a DAG: nothing stops an operation from reaching a
// zero-count it already visited by a different route, or from reaching back the
// zero-count it started from.
internal readonly struct EqualizeStateTopology : IGraphTopology<EqualizeStateNode, ListChildren<EqualizeStateNode>>
{
    public static ListChildren<EqualizeStateNode> GetChildren(EqualizeStateNode node) => new(node.Neighbors);
}
