using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's BitTrie, through its view BitTrieSet, held to IMaxXorSet's contract: it inherits
// every fact MaxXorSetContractTests states, the same facts TwoFieldBitTrie, the hand-specialized
// strategy for that layer, meets. What the view adds is that it holds no state of its own: a value
// inserted through it is in the trie it was made over.
public sealed partial class BitTrieSetTests : MaxXorSetContractTests<BitTrieSet>
{
    private const int InsertedValue = 12;

    [Fact]
    public void Insert_ThroughTheView_IsHeldByTheTrie()
    {
        var trie = new BitTrie();
        new BitTrieSet(trie).Insert(InsertedValue);

        Assert.Equal((1, true), (trie.Count, trie.HasKey(InsertedValue)));
    }
}
