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
// GetOrAddPath is the one Find follows, edge by edge, and Find stops where the path does.
public sealed partial class TrieWalkTests
{
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

    // A test step: one more value passes through each node it reaches.
    private readonly struct CountUp : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count++;
    }
}
