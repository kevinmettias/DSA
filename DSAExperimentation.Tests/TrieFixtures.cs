using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class TrieNode
{
    public TrieNode?[] Children { get; } = new TrieNode?[26];

    public bool IsWord { get; set; }
}

public readonly struct TrieTopology : ITreeTopology<TrieNode, SparseArrayChildren<TrieNode>>
{
    public static SparseArrayChildren<TrieNode> GetChildren(TrieNode node) => new(node.Children);
}

public static class TrieTrees
{
    public static TrieNode FromWords(params string[] words)
    {
        var root = new TrieNode();

        foreach (var word in words)
        {
            var current = root;

            foreach (var ch in word)
            {
                var index = ch - 'a';
                current = current.Children[index] ??= new TrieNode();
            }

            current.IsWord = true;
        }

        return root;
    }
}
