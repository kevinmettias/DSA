using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// PooledCountedBitTrie, the hand-specialized strategy for the XOR multiset, through its view
// PooledCountedBitTrieMultiset, held to IXorMultiset's contract: it inherits every fact
// XorMultisetContractTests states, the same facts the one trie's CountedBitTrie meets (ARCHITECTURE
// §13.9). What the view adds is that it holds no state of its own: a value inserted through it is
// in the trie it was made over.
public sealed partial class PooledCountedBitTrieMultisetTests
    : XorMultisetContractTests<PooledCountedBitTrieMultiset>
{
    private const int InsertedValue = 12;

    [Fact]
    public void Insert_ThroughTheView_IsHeldByTheTrie()
    {
        var trie = new PooledCountedBitTrie();
        new PooledCountedBitTrieMultiset(trie).Insert(InsertedValue);

        Assert.Equal((1, true), (trie.Count, trie.TryRemove(InsertedValue)));
    }
}
