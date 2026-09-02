using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters.Fixtures;

internal readonly struct PathTopology : ITreeTopology<PathNode, ListChildren<PathNode>>
{
    public static ListChildren<PathNode> GetChildren(PathNode node) => new(node.Children);
}
