using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using BitChildren = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNodeChildren<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode>>;
using BitTopology = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNodeTopology<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode>>;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The bit trie's own members beside the set contract: its root, and the tree-topology witness it
// has as a trie over an ordered store. Its set contract is TrieContractTests', run over an int's
// bits in TrieTests, and its maximum-XOR query is MaxXorSetContractTests', run over it through
// BitTrieSet.
public sealed partial class BitTrieTests
{
    // 0 and 1 share every bit but the last: the root, the 31 nodes of their shared chain, and the
    // two leaves where they part.
    private const int NodesOfZeroAndOne = 34;

    [Fact]
    public void Root_IsTheSameNodeAcrossCalls()
    {
        var trie = new BitTrie();

        Assert.Same(trie.Root, trie.Root);
    }

    // The tree-topology witness is real, not just a satisfied interface: TreeMetrics.Size, generic
    // over any ITreeTopology, counts every node with no bit-trie code.
    [Fact]
    public void Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedBitPrefixChain()
    {
        var trie = TrieOf([0, 1]);

        Assert.Equal(NodesOfZeroAndOne, TreeMetrics.Size<BitTrieNode, BitTopology, BitChildren>(trie.Root));
    }

    private static BitTrie TrieOf(int[] values)
    {
        var trie = new BitTrie();

        foreach (var value in values)
        {
            trie.Insert(value);
        }

        return trie;
    }
}
