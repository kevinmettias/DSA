using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.DataStructures.Graph.Hamming;

// A Hamming graph is a general graph, not a DAG: "differs in exactly one
// position" is symmetric, so every edge appears in both directions.
internal readonly struct HammingTopology : IGraphTopology<HammingNode, ListChildren<HammingNode>>
{
    public static ListChildren<HammingNode> GetChildren(HammingNode node) => new(node.Neighbors);
}
