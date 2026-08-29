using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Metrics;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class LowercaseTrieTests
{
    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var trie = new LowercaseTrie<int>();

        trie.Set("cat", 1);
        var found = trie.TryGetValue("cat", out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue_CountUnchanged()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("cat", 1);

        trie.Set("cat", 2);
        trie.TryGetValue("cat", out var value);

        Assert.Equal(2, value);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("cat", 1);

        Assert.True(trie.HasKey("cat"));
        Assert.False(trie.HasKey("dog"));
    }

    [Fact]
    public void TryGetValue_UnknownKey_ReturnsFalseAndDefault()
    {
        var trie = new LowercaseTrie<int>();

        var found = trie.TryGetValue("missing", out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void HasPrefix_KnownPrefix_ReturnsTrue()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("card", 1);

        Assert.True(trie.HasPrefix("car"));
        Assert.True(trie.HasPrefix("card"));
    }

    [Fact]
    public void HasPrefix_UnknownPrefix_ReturnsFalse()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("card", 1);

        Assert.False(trie.HasPrefix("dog"));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsTrueWhenAnyKeyExists()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("card", 1);

        Assert.True(trie.HasPrefix(""));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsFalseWhenEmpty()
    {
        var trie = new LowercaseTrie<int>();

        Assert.False(trie.HasPrefix(""));
    }

    [Fact]
    public void Count_TracksDistinctKeysOnly()
    {
        var trie = new LowercaseTrie<int>();

        trie.Set("cat", 1);
        trie.Set("car", 2);
        trie.Set("card", 3);
        trie.Set("car", 4);

        Assert.Equal(3, trie.Count);
    }

    [Fact]
    public void Set_KeyOutsideLowercaseAlphabet_ThrowsArgumentOutOfRangeException()
    {
        var trie = new LowercaseTrie<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set("Cat", 1));
    }

    [Fact]
    public void Root_ReturnsTheSameNodeAcrossCalls()
    {
        var trie = new LowercaseTrie<int>();

        Assert.Same(trie.Root, trie.Root);
    }

    // Proves the ITreeTopology witness is real, not just a satisfied interface:
    // TreeMetrics.Size (Algorithms/Metrics, generic over ANY ITreeTopology) counts
    // every LowercaseTrieNode reachable from the root with no LowercaseTrie-specific
    // code at all - root + c + a + r + d = 5 nodes for "car"/"card"'s shared-prefix
    // chain, the same SizeAlgebra.Combine = 1 + children.Sum() BinaryTree/Grid already
    // use.
    [Fact]
    public void Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedPrefixChain()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("car", 1);
        trie.Set("card", 2);

        var size = TreeMetrics.Size<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>>(trie.Root);

        Assert.Equal(5, size);
    }

    // Proves the same witness generalizes to a SECOND generic engine, and that the
    // generalization is a genuinely useful one, not just a compiling one: Longest
    // Common Prefix of two keys IS LowestCommonAncestor.Find (Algorithms/Ancestry) over
    // a trie topology - the node where "car" and "cat" diverge is exactly the node
    // reached by walking their shared prefix "ca", with no trie-specific LCA code
    // required.
    [Fact]
    public void LowestCommonAncestor_OfTwoKeys_IsTheNodeAtTheirLongestCommonPrefix()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set("car", 1);
        trie.Set("cat", 2);
        trie.Set("dog", 3);

        var carNode = WalkTo(trie.Root, "car");
        var catNode = WalkTo(trie.Root, "cat");
        var expectedLongestCommonPrefixNode = WalkTo(trie.Root, "ca");

        var lca = LowestCommonAncestor.Find<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>>(trie.Root, carNode, catNode);

        Assert.Same(expectedLongestCommonPrefixNode, lca);
    }

    private static LowercaseTrieNode<int> WalkTo(LowercaseTrieNode<int> root, string key)
    {
        var current = root;

        foreach (var ch in key)
        {
            current = current.Children[ch - 'a']!;
        }

        return current;
    }
}
