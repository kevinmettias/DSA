using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadder.Fixtures;

internal readonly struct WordTopology : IGraphTopology<WordNode, ListChildren<WordNode>>
{
    public static ListChildren<WordNode> GetChildren(WordNode node) => new(node.Neighbors);
}
