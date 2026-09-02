using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Metrics;
using BitTrieOperations = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BitTrie;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class BitTrieTests
{
    // The canonical LeetCode 421 example: nums = [3,10,5,25,2,8], the maximum XOR
    // pair is 5^25 = 28.
    private static readonly int[] ClassicExampleValues = [3, 10, 5, 25, 2, 8];

    [Fact]
    public void TryMaxXor_EmptyTrie_ReturnsFalse()
    {
        const int QueryValue = 5;

        var trie = new BitTrieOperations();

        var found = trie.TryMaxXor(QueryValue, out var result);

        Assert.False(found);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryMaxXor_SingleInsertedValue_ReturnsXorAgainstIt()
    {
        const int InsertedValue = 3;
        const int QueryValue = 5;

        var trie = new BitTrieOperations();
        trie.Insert(InsertedValue);

        var found = trie.TryMaxXor(QueryValue, out var result);

        Assert.True(found);
        Assert.Equal(InsertedValue ^ QueryValue, result);
    }

    [Fact]
    public void TryMaxXor_ClassicExample_FindsMaximumAcrossAllInsertedValues()
    {
        const int ExpectedMaxXor = 28;

        var trie = new BitTrieOperations();

        foreach (var value in ClassicExampleValues)
        {
            trie.Insert(value);
        }

        var best = 0;

        foreach (var query in ClassicExampleValues)
        {
            trie.TryMaxXor(query, out var result);
            best = Math.Max(best, result);
        }

        Assert.Equal(ExpectedMaxXor, best);
    }

    [Fact]
    public void TryMaxXor_PrefersOppositeBitAtEveryLevel_OverNearestValue()
    {
        // 0b0000 and 0b1111: opposite at every one of these 4 bits, so their XOR
        // (0b1111 = 15) beats any pairing with 0b1110 (XOR 1) even though 0b1110
        // is numerically closer to 0b1111 than 0b0000 is.
        const int NearNeighborValue = 0b1110;
        const int FullyOppositeQuery = 0b1111;

        var trie = new BitTrieOperations();
        trie.Insert(0b0000);
        trie.Insert(NearNeighborValue);

        var found = trie.TryMaxXor(FullyOppositeQuery, out var result);

        Assert.True(found);
        Assert.Equal(FullyOppositeQuery, result);
    }

    [Fact]
    public void Insert_DuplicateValue_IncrementsCountForEachCall()
    {
        const int DuplicateValue = 7;
        const int ExpectedCountAfterTwoInserts = 2;

        var trie = new BitTrieOperations();

        trie.Insert(DuplicateValue);
        trie.Insert(DuplicateValue);

        Assert.Equal(ExpectedCountAfterTwoInserts, trie.Count);
    }

    [Fact]
    public void TryMaxXor_ValueXorWithItself_CanReturnZero()
    {
        const int Value = 9;

        var trie = new BitTrieOperations();
        trie.Insert(Value);

        var found = trie.TryMaxXor(Value, out var result);

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
        const int ExpectedNodeCount = 34;

        var trie = new BitTrieOperations();
        trie.Insert(0);
        trie.Insert(1);

        var size = TreeMetrics.Size<
            BitTrieNode, BitTrieTopology, BitTrieChildren,
            NaturalChildOrder<BitTrieNode, BitTrieChildren>,
            BitTrieChildren>(trie.Root);

        Assert.Equal(ExpectedNodeCount, size);
    }
}
