using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Metrics;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class LowercaseTrieTests
{
    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var trie = new LowercaseTrie<int>();

        trie.Set(Fixtures.CatKey, 1);
        var found = trie.TryGetValue(Fixtures.CatKey, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue_CountUnchanged()
    {
        var trie = BuildTrie((Fixtures.CatKey, 1), (Fixtures.CatKey, Fixtures.OverwrittenValue));
        trie.TryGetValue(Fixtures.CatKey, out var value);

        Assert.Equal(Fixtures.OverwrittenValue, value);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        var trie = BuildTrie((Fixtures.CatKey, 1));

        Assert.True(trie.HasKey(Fixtures.CatKey));
        Assert.False(trie.HasKey(Fixtures.DogKey));
    }

    [Fact]
    public void TryGetValue_UnknownKey_ReturnsFalseAndDefault()
    {
        var trie = new LowercaseTrie<int>();

        var found = trie.TryGetValue(Fixtures.MissingKey, out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void HasPrefix_KnownPrefix_ReturnsTrue()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.True(trie.HasPrefix(Fixtures.CarKey));
        Assert.True(trie.HasPrefix(Fixtures.CardKey));
    }

    [Fact]
    public void HasPrefix_UnknownPrefix_ReturnsFalse()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.False(trie.HasPrefix(Fixtures.DogKey));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsTrueWhenAnyKeyExists()
    {
        var trie = BuildTrie((Fixtures.CardKey, 1));

        Assert.True(trie.HasPrefix(Fixtures.EmptyPrefix));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsFalseWhenEmpty()
    {
        var trie = new LowercaseTrie<int>();

        Assert.False(trie.HasPrefix(Fixtures.EmptyPrefix));
    }

    [Fact]
    public void Count_TracksDistinctKeysOnly()
    {
        var trie = BuildTrie(
            (Fixtures.CatKey, 1),
            (Fixtures.CarKey, Fixtures.CarValue),
            (Fixtures.CardKey, Fixtures.CardValue),
            (Fixtures.CarKey, Fixtures.CarOverwrittenValue));

        Assert.Equal(Fixtures.ExpectedDistinctKeyCount, trie.Count);
    }

    [Fact]
    public void Set_KeyOutsideLowercaseAlphabet_ThrowsArgumentOutOfRangeException()
    {
        var trie = new LowercaseTrie<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => trie.Set(Fixtures.UppercaseCatKey, 1));
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
        var trie = BuildTrie((Fixtures.CarKey, 1), (Fixtures.CardKey, Fixtures.CardValueForSizeTest));

        var size = TreeMetrics.Size<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>>(trie.Root);

        Assert.Equal(Fixtures.ExpectedTreeSize, size);
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
        var trie = BuildTrie(
            (Fixtures.CarKey, 1),
            (Fixtures.CatKey, Fixtures.CatValueForLcaTest),
            (Fixtures.DogKey, Fixtures.DogValueForLcaTest));

        var carNode = WalkTo(trie.Root, Fixtures.CarKey);
        var catNode = WalkTo(trie.Root, Fixtures.CatKey);
        var sharedPrefixNode = WalkTo(trie.Root, Fixtures.SharedPrefixOfCarAndCat);

        var lca = LowestCommonAncestor.Find<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>>(
                trie.Root, carNode, catNode);

        Assert.Same(sharedPrefixNode, lca);
    }

    // The trie a fixture test starts from: the key/value Sets to replay, in order. One
    // builder rather than one per test, because every fixture in this file is the same
    // thing - a trie built by setting some keys - and the call site is where the keys
    // that matter to a particular test belong.
    private static LowercaseTrie<int> BuildTrie(params (string Key, int Value)[] entries)
    {
        var trie = new LowercaseTrie<int>();

        foreach (var (key, value) in entries)
        {
            trie.Set(key, value);
        }

        return trie;
    }

    // Every key walked here is a prefix of a key this file has already inserted - "car",
    // "cat", or the "ca" the two share - so each child along the path was created by that
    // insert and is present by construction.
    private static LowercaseTrieNode<int> WalkTo(LowercaseTrieNode<int> root, string key)
    {
        var current = root;

        foreach (var ch in key)
        {
            current = current.Children[ch - 'a']
                ?? throw new InvalidOperationException($"WalkTo was given '{key}', which is not a prefix of any key in this trie");
        }

        return current;
    }

    /// <summary>
    /// The keys these tests insert, and the values and counts they expect back, named
    /// once so a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string CatKey = "cat";
        public const string DogKey = "dog";
        public const string CarKey = "car";
        public const string CardKey = "card";
        public const string MissingKey = "missing";
        public const string EmptyPrefix = "";
        public const string UppercaseCatKey = "Cat";
        public const string SharedPrefixOfCarAndCat = "ca";

        public const int OverwrittenValue = 2;
        public const int CarValue = 2;
        public const int CardValue = 3;
        public const int CarOverwrittenValue = 4;
        public const int ExpectedDistinctKeyCount = 3;
        public const int CardValueForSizeTest = 2;
        public const int ExpectedTreeSize = 5;
        public const int CatValueForLcaTest = 2;
        public const int DogValueForLcaTest = 3;
    }
}
