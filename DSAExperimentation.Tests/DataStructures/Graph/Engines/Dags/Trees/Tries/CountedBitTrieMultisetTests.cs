using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's CountedBitTrie, through its view CountedBitTrieMultiset, held to IXorMultiset's
// contract: it inherits every fact XorMultisetContractTests states, the same facts
// PooledCountedBitTrie, the hand-specialized strategy for that layer, meets. What the view adds is
// that it holds no state of its own: a value inserted through it is in the trie it was made over.
public sealed partial class CountedBitTrieMultisetTests
    : XorMultisetContractTests<CountedBitTrieMultiset>
{
    private const int InsertedValue = 12;

    [Fact]
    public void Insert_ThroughTheView_IsHeldByTheTrie()
    {
        var trie = new CountedBitTrie();
        new CountedBitTrieMultiset(trie).Insert(InsertedValue);

        Assert.Equal((1, true), (trie.Count, trie.TryRemove(InsertedValue)));
    }
}
