using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.LoudAndRich;

// Edges point richer -> poorer, so a topological order is richest-first and the
// answer DP can be threaded down every edge in one pass.
internal readonly struct PersonTopology : IGraphTopology<PersonNode, ListChildren<PersonNode>>
{
    public static ListChildren<PersonNode> GetChildren(PersonNode node) => new(node.Poorer);
}
