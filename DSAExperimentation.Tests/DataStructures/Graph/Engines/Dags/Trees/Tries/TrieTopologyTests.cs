using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using WordChildren = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieChildren<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;
using WordNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;
using WordTopology = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieTopology<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;
using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class TrieTopologyTests
{
    private const int Z = 25;
    private const int B = 1;

    // A dense node allocates a slot for every letter; only the filled ones are children.
    [Fact]
    public void GetChildren_EmptyNode_ReturnsNone()
    {
        var dense = TrieTopology<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>.GetChildren(new());
        var packed = TrieTopology<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>.GetChildren(new());

        Assert.Equal(0, dense.Count);
        Assert.Equal(0, packed.Count);
    }

    [Fact]
    public void GetChildren_ListsTheNodesChildrenInLetterOrder()
    {
        var dense = new TrieNode<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>();
        var packed = new TrieNode<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>();
        var denseZ = dense.Children.GetOrAddChild(Z);
        var denseB = dense.Children.GetOrAddChild(B);
        var packedZ = packed.Children.GetOrAddChild(Z);
        var packedB = packed.Children.GetOrAddChild(B);

        var denseChildren = TrieTopology<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>.GetChildren(dense);
        var packedChildren = TrieTopology<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>.GetChildren(packed);

        Assert.Same(denseB, denseChildren.Get(0));
        Assert.Same(denseZ, denseChildren.Get(1));
        Assert.Same(packedB, packedChildren.Get(0));
        Assert.Same(packedZ, packedChildren.Get(1));
    }

    // The topology witness is real, not just a satisfied interface: TreeMetrics.Size, generic over any
    // ITreeTopology, counts every node from the root with no trie code - root + c + a + r + d = 5 for
    // "car" and "card"'s shared chain.
    [Fact]
    public void Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedPrefixChain()
    {
        var trie = WordTrieOf("car", "card");

        Assert.Equal(5, TreeMetrics.Size<WordNode, WordTopology, WordChildren>(trie.Root));
    }

    // The longest common prefix of two keys is LowestCommonAncestor.Find over the trie's topology: the
    // node where "car" and "cat" diverge is the node "ca" reaches, with no trie-specific code.
    [Fact]
    public void LowestCommonAncestor_OfTwoKeys_IsTheNodeAtTheirLongestCommonPrefix()
    {
        var trie = WordTrieOf("car", "cat", "dog");

        var lca = LowestCommonAncestor.Find<WordNode, WordTopology, WordChildren>(trie.Root, WalkTo(trie.Root, "car"), WalkTo(trie.Root, "cat"));

        Assert.Same(WalkTo(trie.Root, "ca"), lca);
    }

    private static WordTrie WordTrieOf(params string[] keys)
    {
        var trie = new WordTrie();

        foreach (var key in keys)
        {
            trie.Set(key, 1);
        }

        return trie;
    }

    // Every key walked here is a prefix of a key the test inserted, so each child on its path exists.
    private static WordNode WalkTo(WordNode root, string key)
    {
        var current = root;

        foreach (var ch in key)
        {
            current = current.Children.ChildAt(EnglishLowercase.IndexOf(ch))
                ?? throw new InvalidOperationException($"WalkTo was given '{key}', which is not a prefix of any key in this trie");
        }

        return current;
    }
}
