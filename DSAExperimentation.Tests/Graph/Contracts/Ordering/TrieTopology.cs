using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.Graph.Contracts.Ordering;

internal readonly struct TrieTopology : ITreeTopology<TrieNode, SparseArrayChildren<TrieNode>>
{
    public static SparseArrayChildren<TrieNode> GetChildren(TrieNode node) => new(node.Children);
}
