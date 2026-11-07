using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using BitWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode>,
    int>;
using CountedWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode>,
    int>;
using SuffixWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.SuffixTreeNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.HashedNodeChildren<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.SuffixTreeNode>,
    char>;
using WordNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    int>;
using WordWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
            DSAExperimentation.Domain.Language.English.EnglishLowercase,
            DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
            int>,
        int>,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        int>,
    char>;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The walk over two node classes, a word node with a value and a bit node with none: a path made by
// GetOrAddPath is the one Find follows, edge by edge, and Find stops where the path does. The
// edge-label walk runs over a compressed trie of "banana" built by hand, its edges ranges of the
// text, and over the word trie with one letter per edge, where it must find what Find finds.
public sealed partial class TrieWalkTests
{
    private const string Banana = "banana";

    public static TheoryData<string> WordKeys =>
        new() { "", "t", "tr", "tre", "tree", "trees", "to", "x" };

    public static TheoryData<string> KeysLeavingAnEdge =>
        new() { "bad", "banane", "nanb", "nab", "bananas" };
    [Fact]
    public void GetOrAddPath_WalksOneEdgePerLetter()
    {
        var root = new WordNode();
        var end = WordWalk.GetOrAddPath(root, new TextKey("ab"));

        var a = root.Children.ChildAt(EnglishLowercase.IndexOf('a'));
        Assert.Same(end, a?.Children.ChildAt(EnglishLowercase.IndexOf('b')));
    }

    [Fact]
    public void GetOrAddPath_PathAlreadyThere_AddsNothing()
    {
        var root = new WordNode();
        var first = WordWalk.GetOrAddPath(root, new TextKey("ab"));

        Assert.Same(first, WordWalk.GetOrAddPath(root, new TextKey("ab")));
    }

    [Fact]
    public void Find_KeyOfNoLetters_IsTheRoot()
    {
        var root = new WordNode();

        Assert.Same(root, WordWalk.Find(root, new TextKey(string.Empty)));
    }

    [Fact]
    public void Find_PathThatStopsShort_IsNull()
    {
        var root = new WordNode();
        WordWalk.GetOrAddPath(root, new TextKey("ab"));

        Assert.Null(WordWalk.Find(root, new TextKey("abc")));
    }

    [Fact]
    public void Find_BitNode_FollowsThePathGetOrAddPathMade()
    {
        var root = new BitTrieNode();
        var end = BitWalk.GetOrAddPath(root, new Int32BitsKey(5));

        Assert.Same(end, BitWalk.Find(root, new Int32BitsKey(5)));
        Assert.Null(BitWalk.Find(root, new Int32BitsKey(4)));
    }

    // 4 and 5 share their first 31 bits: the step reaches the root and that shared chain on both
    // walks, and each leaf on its own walk only.
    [Fact]
    public void GetOrAddPath_WithAStep_AppliesItToEveryNodeOnThePathFromTheRoot()
    {
        var root = new CountedBitTrieNode();
        var four = CountedWalk.GetOrAddPath<Int32BitsKey, CountUp>(root, new Int32BitsKey(4));
        var five = CountedWalk.GetOrAddPath<Int32BitsKey, CountUp>(root, new Int32BitsKey(5));
        var sharedBits = new KeyPrefix<Int32BitsKey, int>(new Int32BitsKey(4), 31);
        var shared = CountedWalk.Find(root, sharedBits);

        Assert.Equal((2, 2, 1, 1), (root.Count, shared?.Count, four.Count, five.Count));
    }

    // "ban" stops inside the edge "banana" and "nan" inside the second "na" below the first: each
    // is a prefix of every suffix below that edge.
    [Fact]
    public void FindAlongEdges_KeyEndingPartwayAlongAnEdge_IsTheNodeBelowIt()
    {
        var trie = BananaTrie.Build();
        var edges = new SuffixTreeEdges(Banana);

        Assert.Same(trie.BananaEdge, SuffixWalk.Find(trie.Root, "ban", edges));
        Assert.Same(trie.NanaEdge, SuffixWalk.Find(trie.Root, "nan", edges));
    }

    [Fact]
    public void FindAlongEdges_KeyEndingAtAnEdgesEnd_IsTheNodeBelowIt()
    {
        var trie = BananaTrie.Build();

        var found = SuffixWalk.Find(trie.Root, "na", new SuffixTreeEdges(Banana));

        Assert.Same(trie.NaEdge, found);
    }

    // "bad" leaves its edge at the third letter, "banane" and "nanb" at an edge's last letter,
    // "nab" where a node has no child, and "bananas" past a leaf.
    [Theory]
    [MemberData(nameof(KeysLeavingAnEdge))]
    public void FindAlongEdges_KeyLeavingTheTrie_IsNull(string key)
    {
        var root = BananaTrie.Build().Root;

        Assert.Null(SuffixWalk.Find(root, key, new SuffixTreeEdges(Banana)));
    }

    [Fact]
    public void FindAlongEdges_KeyOfNoLetters_IsTheRoot()
    {
        var root = BananaTrie.Build().Root;

        var found = SuffixWalk.Find(root, string.Empty, new SuffixTreeEdges(Banana));

        Assert.Same(root, found);
    }

    // One letter per edge is the uncompressed trie: the edge-label walk then matches nothing past
    // the store's lookup, and ends where the plain walk does.
    [Theory]
    [MemberData(nameof(WordKeys))]
    public void FindAlongEdges_OneLetterPerEdge_FindsWhatFindFinds(string key)
    {
        var root = new WordNode();

        foreach (var word in new[] { "tree", "trie", "to" })
        {
            WordWalk.GetOrAddPath(root, new TextKey(word));
        }

        var plain = WordWalk.Find(root, new TextKey(key));

        Assert.Same(plain, WordWalk.Find(root, key, new OneLetterEdges()));
    }

    // A test step: one more value passes through each node it reaches.
    private readonly struct CountUp : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count++;
    }

    // The uncompressed trie's labels, a test's own: every edge is the one letter the store has
    // already matched, so the walk never asks for another.
    private readonly struct OneLetterEdges : ITrieEdgeLabels<WordNode, char>
    {
        public int LengthOf(WordNode node) => 1;

        public bool HasLetterAt(WordNode node, int offset, char letter) => false;
    }

    // A compressed trie over "banana" by hand: "banana" and "na" below the root, and "na" again
    // below "na", each edge a range of the text.
    private readonly record struct BananaTrie(
        SuffixTreeNode Root,
        SuffixTreeNode BananaEdge,
        SuffixTreeNode NaEdge,
        SuffixTreeNode NanaEdge)
    {
        public static BananaTrie Build()
        {
            var root = new SuffixTreeNode();
            var banana = Edge(root, 0, Banana.Length);
            var na = Edge(root, 2, 2);
            var nana = Edge(na, 4, 2);

            return new BananaTrie(root, banana, na, nana);
        }

        private static SuffixTreeNode Edge(SuffixTreeNode parent, int start, int length)
        {
            var child = parent.Children.GetOrAddChildFor(Banana[start]);
            child.Start = start;
            child.Length = length;

            return child;
        }
    }
}
