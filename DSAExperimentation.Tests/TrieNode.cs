namespace DSAExperimentation.Tests;

public sealed class TrieNode
{
    public TrieNode?[] Children { get; } = new TrieNode?[26];

    public bool IsWord { get; set; }
}
