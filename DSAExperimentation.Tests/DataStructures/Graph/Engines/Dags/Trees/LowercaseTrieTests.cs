using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Metrics;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class LowercaseTrieTests
{
    private const string CatKey = "cat";
    private const string DogKey = "dog";
    private const string CarKey = "car";
    private const string CardKey = "card";
    private const string MissingKey = "missing";
    private const string EmptyPrefix = "";
    private const string UppercaseCatKey = "Cat";
    private const string SharedPrefixOfCarAndCat = "ca";

    private const int OverwrittenValue = 2;
    private const int CarValue = 2;
    private const int CardValue = 3;
    private const int CarOverwrittenValue = 4;
    private const int ExpectedDistinctKeyCount = 3;
    private const int CardValueForSizeTest = 2;
    private const int ExpectedTreeSize = 5;
    private const int CatValueForLcaTest = 2;
    private const int DogValueForLcaTest = 3;

    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var trie = new LowercaseTrie<int>();

        trie.Set(CatKey, 1);
        var found = trie.TryGetValue(CatKey, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue_CountUnchanged()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set(CatKey, 1);

        trie.Set(CatKey, OverwrittenValue);
        trie.TryGetValue(CatKey, out var value);

        Assert.Equal(OverwrittenValue, value);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set(CatKey, 1);

        Assert.True(trie.HasKey(CatKey));
        Assert.False(trie.HasKey(DogKey));
    }

    [Fact]
    public void TryGetValue_UnknownKey_ReturnsFalseAndDefault()
    {
        var trie = new LowercaseTrie<int>();

        var found = trie.TryGetValue(MissingKey, out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void HasPrefix_KnownPrefix_ReturnsTrue()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set(CardKey, 1);

        Assert.True(trie.HasPrefix(CarKey));
        Assert.True(trie.HasPrefix(CardKey));
    }

    [Fact]
    public void HasPrefix_UnknownPrefix_ReturnsFalse()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set(CardKey, 1);

        Assert.False(trie.HasPrefix(DogKey));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsTrueWhenAnyKeyExists()
    {
        var trie = new LowercaseTrie<int>();
        trie.Set(CardKey, 1);

        Assert.True(trie.HasPrefix(EmptyPrefix));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsFalseWhenEmpty()
    {
        var trie = new LowercaseTrie<int>();

        Assert.False(trie.HasPrefix(EmptyPrefix));
    }

    [Fact]
    public void Count_TracksDistinctKeysOnly()
    {
        var trie = new LowercaseTrie<int>();

        trie.Set(CatKey, 1);
        trie.Set(CarKey, CarValue);
        trie.Set(CardKey, CardValue);
        trie.Set(CarKey, CarOverwrittenValue);

        Assert.Equal(ExpectedDistinctKeyCount, trie.Count);
    }

    [Fact]
    public void Set_KeyOutsideLowercaseAlphabet_ThrowsArgumentOutOfRangeException()
    {
        var trie = new LowercaseTrie<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set(UppercaseCatKey, 1));
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
        trie.Set(CarKey, 1);
        trie.Set(CardKey, CardValueForSizeTest);

        var size = TreeMetrics.Size<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>>(trie.Root);

        Assert.Equal(ExpectedTreeSize, size);
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
        trie.Set(CarKey, 1);
        trie.Set(CatKey, CatValueForLcaTest);
        trie.Set(DogKey, DogValueForLcaTest);

        var carNode = WalkTo(trie.Root, CarKey);
        var catNode = WalkTo(trie.Root, CatKey);
        var expectedLongestCommonPrefixNode = WalkTo(trie.Root, SharedPrefixOfCarAndCat);

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
