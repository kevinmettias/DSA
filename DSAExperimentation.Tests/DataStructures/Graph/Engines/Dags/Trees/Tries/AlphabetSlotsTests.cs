using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Every fact runs under both layouts: a node's answers are the same whichever layout stores its
// children, and the packed layout's shifting and growing is what most needs proving. A child's Value is
// set to its letter's slot as it is added, so a child read back names the letter it was stored under.
public sealed partial class AlphabetSlotsTests
{
    private const int NotASlot = 26;

    // Bits 2 and 4: letters 2 and 4 have children.
    private const ulong LettersTwoAndFour = 0b1_0100;

    public static TheoryData<int[]> InsertionOrders => new()
    {
        { [2, 0, 25, 1] },
        { [3, 3, 1, 3] },
        { [.. Enumerable.Range(0, EnglishLetters.Size).Reverse()] },
        { [0, 25, 1, 24, 2, 23, 3, 22, 4, 21, 5, 20, 6, 19, 7, 18, 8, 17, 9, 16, 10, 15, 11, 14, 12, 13] },
    };

    [Theory]
    [MemberData(nameof(InsertionOrders))]
    public void ChildAt_AfterAnyInsertionOrder_FindsEachLettersOwnChild(int[] letters)
    {
        var dense = NodeWithChildren<DenseTrieLayout>(letters);
        var packed = NodeWithChildren<PackedTrieLayout>(letters);

        Assert.All(letters, letter => Assert.Equal(letter, ValueOfChildAt(dense, letter)));
        Assert.All(letters, letter => Assert.Equal(letter, ValueOfChildAt(packed, letter)));
    }

    [Theory]
    [MemberData(nameof(InsertionOrders))]
    public void ChildOfRank_AfterAnyInsertionOrder_ListsChildrenInLetterOrder(int[] letters)
    {
        var expected = letters.Distinct().Order().ToArray();
        var dense = NodeWithChildren<DenseTrieLayout>(letters);
        var packed = NodeWithChildren<PackedTrieLayout>(letters);

        Assert.Equal(expected, ValuesByRank(dense));
        Assert.Equal(expected, ValuesByRank(packed));
    }

    [Theory]
    [MemberData(nameof(InsertionOrders))]
    public void Count_CountsDistinctLetters(int[] letters)
    {
        var expected = letters.Distinct().Count();

        Assert.Equal(expected, NodeWithChildren<DenseTrieLayout>(letters).Children.Count);
        Assert.Equal(expected, NodeWithChildren<PackedTrieLayout>(letters).Children.Count);
    }

    [Fact]
    public void Letters_HasABitForEachLetterWithAChild()
    {
        Assert.Equal(LettersTwoAndFour, NodeWithChildren<DenseTrieLayout>(4, 2).Children.Letters);
        Assert.Equal(LettersTwoAndFour, NodeWithChildren<PackedTrieLayout>(4, 2).Children.Letters);
    }

    [Theory]
    [MemberData(nameof(InsertionOrders))]
    public void GetEnumerator_AfterAnyInsertionOrder_GivesEachChildWithTheLetterItIsStoredUnder(int[] letters)
    {
        var dense = NodeWithChildren<DenseTrieLayout>(letters);
        var packed = NodeWithChildren<PackedTrieLayout>(letters);

        Assert.All(LettersAndValues(dense), pair => Assert.Equal(pair.Letter, pair.Value));
        Assert.All(LettersAndValues(packed), pair => Assert.Equal(pair.Letter, pair.Value));
    }

    [Fact]
    public void ChildAt_LetterWithNoChild_ReturnsNull()
    {
        var dense = NodeWithChildren<DenseTrieLayout>(2, 4);
        var packed = NodeWithChildren<PackedTrieLayout>(2, 4);

        Assert.Null(dense.Children.ChildAt(3));
        Assert.Null(packed.Children.ChildAt(3));
        Assert.Null(packed.Children.ChildAt(25));
    }

    [Fact]
    public void ChildAt_NotASlotOfTheAlphabet_Throws()
    {
        var dense = NodeWithChildren<DenseTrieLayout>(0);
        var packed = NodeWithChildren<PackedTrieLayout>(0);

        Assert.Throws<ArgumentOutOfRangeException>(() => dense.Children.ChildAt(NotASlot));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.Children.ChildAt(NotASlot));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.Children.ChildAt(-1));
    }

    [Fact]
    public void ChildOfRank_AtTheChildCount_Throws()
    {
        var dense = NodeWithChildren<DenseTrieLayout>(1, 5);
        var packed = NodeWithChildren<PackedTrieLayout>(1, 5);

        Assert.Throws<ArgumentOutOfRangeException>(() => dense.Children.ChildOfRank(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.Children.ChildOfRank(2));
    }

    [Fact]
    public void GetOrAddChild_SameLetterTwice_ReturnsTheSameChild()
    {
        var dense = new TrieNode<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>();
        var packed = new TrieNode<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>();

        Assert.Same(dense.Children.GetOrAddChild(7), dense.Children.GetOrAddChild(7));
        Assert.Same(packed.Children.GetOrAddChild(7), packed.Children.GetOrAddChild(7));
    }

    [Fact]
    public void GetOrAddChild_NotASlotOfTheAlphabet_Throws()
    {
        var packed = new TrieNode<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => packed.Children.GetOrAddChild(NotASlot));
    }

    private static TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int> NodeWithChildren<TLayout>(params int[] letters)
        where TLayout : struct, ITrieLayout
    {
        var node = new TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int>();

        foreach (var letter in letters)
        {
            node.Children.GetOrAddChild(letter).Value = letter;
        }

        return node;
    }

    private static int ValueOfChildAt<TLayout>(TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int> node, int letter)
        where TLayout : struct, ITrieLayout
    {
        var child = node.Children.ChildAt(letter);
        Assert.NotNull(child);

        return child.Value;
    }

    private static List<(int Letter, int Value)> LettersAndValues<TLayout>(TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int> node)
        where TLayout : struct, ITrieLayout
    {
        var pairs = new List<(int, int)>();

        foreach (var (letter, child) in node.Children)
        {
            pairs.Add((letter, child.Value));
        }

        return pairs;
    }

    private static int[] ValuesByRank<TLayout>(TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int> node)
        where TLayout : struct, ITrieLayout
        => [.. Enumerable.Range(0, node.Children.Count).Select(rank => node.Children.ChildOfRank(rank).Value)];
}
