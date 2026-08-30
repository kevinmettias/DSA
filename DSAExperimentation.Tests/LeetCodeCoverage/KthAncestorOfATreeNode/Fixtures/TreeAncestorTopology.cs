using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthAncestorOfATreeNode.Fixtures;

internal readonly struct TreeAncestorTopology : ITreeTopology<TreeAncestorNode, ListChildren<TreeAncestorNode>>
{
    public static ListChildren<TreeAncestorNode> GetChildren(TreeAncestorNode node) => new(node.Children);
}
