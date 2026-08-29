using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Metrics;
using BitTrieOperations = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BitTrie;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class BitTrieTests
{
    [Fact]
    public void TryMaxXor_EmptyTrie_ReturnsFalse()
    {
        var trie = new BitTrieOperations();

        var found = trie.TryMaxXor(5, out var result);

        Assert.False(found);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryMaxXor_SingleInsertedValue_ReturnsXorAgainstIt()
    {
        var trie = new BitTrieOperations();
        trie.Insert(3);

        var found = trie.TryMaxXor(5, out var result);

        Assert.True(found);
        Assert.Equal(3 ^ 5, result);
    }

    // The canonical LeetCode 421 example: nums = [3,10,5,25,2,8], the maximum XOR
    // pair is 5^25 = 28.
    [Fact]
    public void TryMaxXor_ClassicExample_FindsMaximumAcrossAllInsertedValues()
    {
        var trie = new BitTrieOperations();

        foreach (var value in new[] { 3, 10, 5, 25, 2, 8 })
        {
            trie.Insert(value);
        }

        var best = 0;

        foreach (var query in new[] { 3, 10, 5, 25, 2, 8 })
        {
            trie.TryMaxXor(query, out var result);
            best = Math.Max(best, result);
        }

        Assert.Equal(28, best);
    }

    [Fact]
    public void TryMaxXor_PrefersOppositeBitAtEveryLevel_OverNearestValue()
    {
        // 0b0000 and 0b1111: opposite at every one of these 4 bits, so their XOR
        // (0b1111 = 15) beats any pairing with 0b1110 (XOR 1) even though 0b1110
        // is numerically closer to 0b1111 than 0b0000 is.
        var trie = new BitTrieOperations();
        trie.Insert(0b0000);
        trie.Insert(0b1110);

        var found = trie.TryMaxXor(0b1111, out var result);

        Assert.True(found);
        Assert.Equal(0b1111, result);
    }

    [Fact]
    public void Insert_DuplicateValue_IncrementsCountForEachCall()
    {
        var trie = new BitTrieOperations();

        trie.Insert(7);
        trie.Insert(7);

        Assert.Equal(2, trie.Count);
    }

    [Fact]
    public void TryMaxXor_ValueXorWithItself_CanReturnZero()
    {
        var trie = new BitTrieOperations();
        trie.Insert(9);

        var found = trie.TryMaxXor(9, out var result);

        Assert.True(found);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Root_ReturnsTheSameNodeAcrossCalls()
    {
        var trie = new BitTrieOperations();

        Assert.Same(trie.Root, trie.Root);
    }

    // Proves the ITreeTopology witness is real, not just a satisfied interface:
    // TreeMetrics.Size (Algorithms/Metrics, generic over ANY ITreeTopology) counts
    // every BitTrieNode reachable from the root with no BitTrie-specific code at
    // all. 0 and 1 share every one of their 32 bits except the last (bit 0), so
    // Insert walks the SAME 31-node chain for both before diverging only at the
    // final level: 1 (root) + 31 (shared chain) + 2 (the two distinct leaves where
    // they diverge) = 34 nodes.
    [Fact]
    public void Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedBitPrefixChain()
    {
        var trie = new BitTrieOperations();
        trie.Insert(0);
        trie.Insert(1);

        var size = TreeMetrics.Size<
            BitTrieNode, BitTrieTopology, BitTrieChildren,
            NaturalChildOrder<BitTrieNode, BitTrieChildren>,
            BitTrieChildren>(trie.Root);

        Assert.Equal(34, size);
    }
}
