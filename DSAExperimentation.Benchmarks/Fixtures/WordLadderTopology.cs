using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.Fixtures;

internal readonly struct WordLadderTopology : IGraphTopology<WordLadderNode, ListChildren<WordLadderNode>>
{
    public static ListChildren<WordLadderNode> GetChildren(WordLadderNode node) => new(node.Neighbors);
}
