using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The pooled counted bit trie's own members beside the XOR multiset contract, which
// XorMultisetContractTests holds it to through PooledCountedBitTrieMultiset: the refusal of a copy
// past int.MaxValue, reached by giving the root that count, since inserting 2^31 values would take
// minutes.
public sealed partial class PooledCountedBitTrieTests
{
    private const int InsertedValue = 5;

    [Fact]
    public void Insert_AtIntMaxValueCopies_ThrowsAndKeepsTheCount()
    {
        var trie = new PooledCountedBitTrie();
        trie.SeedRootCount(int.MaxValue);

        Assert.Throws<OverflowException>(() => trie.Insert(InsertedValue));
        Assert.Equal(int.MaxValue, trie.Count);
    }

    // One below the limit, the copy is still taken: the refusal is at int.MaxValue, not before it.
    [Fact]
    public void Insert_OneBelowIntMaxValueCopies_TakesTheCopy()
    {
        var trie = new PooledCountedBitTrie();
        trie.SeedRootCount(int.MaxValue - 1);

        trie.Insert(InsertedValue);

        Assert.Equal(int.MaxValue, trie.Count);
    }
}
