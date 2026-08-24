using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct TrieTopology : ITreeTopology<TrieNode, SparseArrayChildren<TrieNode>>
{
    public static SparseArrayChildren<TrieNode> GetChildren(TrieNode node) => new(node.Children);
}
