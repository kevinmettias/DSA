namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

internal sealed class TrieNode
{
    public TrieNode?[] Children { get; } = new TrieNode?[26];

    public bool IsWord { get; set; }
}
