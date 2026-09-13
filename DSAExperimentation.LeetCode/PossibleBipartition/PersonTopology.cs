using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.PossibleBipartition;

// "Dislike" is mutual, so the edge relation is undirected and may well be cyclic
// - an odd cycle of dislikes is precisely the answer "false" - and nothing here
// promises acyclicity, matching IsGraphBipartite's own BipartiteTopology.
internal readonly struct PersonTopology : IGraphTopology<PersonNode, ListChildren<PersonNode>>
{
    public static ListChildren<PersonNode> GetChildren(PersonNode node) => new(node.Dislikes);
}
