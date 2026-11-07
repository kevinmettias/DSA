using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// TwoFieldBitTrie, the hand-specialized strategy for the maximum-XOR set, held to IMaxXorSet's
// contract: it inherits every fact MaxXorSetContractTests states, the same facts the one trie's
// BitTrie meets through BitTrieSet (ARCHITECTURE §13.9). What it adds is that the set is a struct
// over shared contents, so a copy inserts into the same set.
public sealed partial class TwoFieldBitTrieTests : MaxXorSetContractTests<TwoFieldBitTrie>
{
    private const int InsertedValue = 12;

    [Fact]
    public void Insert_ThroughACopy_IsHeldByTheOriginal()
    {
        var set = TwoFieldBitTrie.Empty();
        var copy = set;
        copy.Insert(InsertedValue);

        Assert.Equal((1, true), (set.Count, set.HasKey(InsertedValue)));
    }
}
