using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Every fact runs under both layouts: a node's answers are the same whichever layout stores its
// children, and the packed layout's shifting and growing is what most needs proving. A child's Value is
// set to its letter's slot as it is added, so a child read back names the letter it was stored under.
public sealed partial class AlphabetTrieNodeTests
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
    public void ChildCount_CountsDistinctLetters(int[] letters)
    {
        var expected = letters.Distinct().Count();

        Assert.Equal(expected, NodeWithChildren<DenseTrieLayout>(letters).ChildCount);
        Assert.Equal(expected, NodeWithChildren<PackedTrieLayout>(letters).ChildCount);
    }

    [Fact]
    public void ChildLetters_HasABitForEachLetterWithAChild()
    {
        Assert.Equal(LettersTwoAndFour, NodeWithChildren<DenseTrieLayout>(4, 2).ChildLetters);
        Assert.Equal(LettersTwoAndFour, NodeWithChildren<PackedTrieLayout>(4, 2).ChildLetters);
    }

    [Theory]
    [MemberData(nameof(InsertionOrders))]
    public void Children_AfterAnyInsertionOrder_GivesEachChildWithTheLetterItIsStoredUnder(int[] letters)
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

        Assert.Null(dense.ChildAt(3));
        Assert.Null(packed.ChildAt(3));
        Assert.Null(packed.ChildAt(25));
    }

    [Fact]
    public void ChildAt_NotASlotOfTheAlphabet_Throws()
    {
        var dense = NodeWithChildren<DenseTrieLayout>(0);
        var packed = NodeWithChildren<PackedTrieLayout>(0);

        Assert.Throws<ArgumentOutOfRangeException>(() => dense.ChildAt(NotASlot));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.ChildAt(NotASlot));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.ChildAt(-1));
    }

    [Fact]
    public void ChildOfRank_AtTheChildCount_Throws()
    {
        var dense = NodeWithChildren<DenseTrieLayout>(1, 5);
        var packed = NodeWithChildren<PackedTrieLayout>(1, 5);

        Assert.Throws<ArgumentOutOfRangeException>(() => dense.ChildOfRank(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => packed.ChildOfRank(2));
    }

    [Fact]
    public void GetOrAddChild_SameLetterTwice_ReturnsTheSameChild()
    {
        var dense = new AlphabetTrieNode<EnglishLowercase, DenseTrieLayout, int>();
        var packed = new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>();

        Assert.Same(dense.GetOrAddChild(7), dense.GetOrAddChild(7));
        Assert.Same(packed.GetOrAddChild(7), packed.GetOrAddChild(7));
    }

    [Fact]
    public void GetOrAddChild_NotASlotOfTheAlphabet_Throws()
    {
        var packed = new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => packed.GetOrAddChild(NotASlot));
    }

    [Fact]
    public void HasValue_NewNode_IsFalse() =>
        Assert.False(new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>().HasValue);

    private static AlphabetTrieNode<EnglishLowercase, TLayout, int> NodeWithChildren<TLayout>(params int[] letters)
        where TLayout : struct, ITrieLayout
    {
        var node = new AlphabetTrieNode<EnglishLowercase, TLayout, int>();

        foreach (var letter in letters)
        {
            node.GetOrAddChild(letter).Value = letter;
        }

        return node;
    }

    private static int ValueOfChildAt<TLayout>(AlphabetTrieNode<EnglishLowercase, TLayout, int> node, int letter)
        where TLayout : struct, ITrieLayout
    {
        var child = node.ChildAt(letter);
        Assert.NotNull(child);

        return child.Value;
    }

    private static List<(int Letter, int Value)> LettersAndValues<TLayout>(AlphabetTrieNode<EnglishLowercase, TLayout, int> node)
        where TLayout : struct, ITrieLayout
    {
        var pairs = new List<(int, int)>();

        foreach (var (letter, child) in node.Children)
        {
            pairs.Add((letter, child.Value));
        }

        return pairs;
    }

    private static int[] ValuesByRank<TLayout>(AlphabetTrieNode<EnglishLowercase, TLayout, int> node)
        where TLayout : struct, ITrieLayout
        => [.. Enumerable.Range(0, node.ChildCount).Select(rank => node.ChildOfRank(rank).Value)];
}
