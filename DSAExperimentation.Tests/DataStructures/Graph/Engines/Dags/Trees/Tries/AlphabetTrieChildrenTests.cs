using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class AlphabetTrieChildrenTests
{
    private const int C = 2;
    private const int A = 0;

    [Fact]
    public void Count_EmptyNode_IsZero()
    {
        var dense = new AlphabetTrieChildren<EnglishLowercase, DenseTrieLayout, int>(new());
        var packed = new AlphabetTrieChildren<EnglishLowercase, PackedTrieLayout, int>(new());

        Assert.Equal(0, dense.Count);
        Assert.Equal(0, packed.Count);
    }

    [Fact]
    public void Get_ListsChildrenInLetterOrder_NotInsertionOrder()
    {
        AssertLetterOrder(new AlphabetTrieNode<EnglishLowercase, DenseTrieLayout, int>());
        AssertLetterOrder(new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>());
    }

    [Fact]
    public void GetEnumerator_GivesTheChildrenInLetterOrderWithTheirLetters()
    {
        var node = new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>();
        var c = node.GetOrAddChild(C);
        var a = node.GetOrAddChild(A);
        var steps = new List<(int, AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>)>();

        foreach (var step in new AlphabetTrieChildren<EnglishLowercase, PackedTrieLayout, int>(node))
        {
            steps.Add(step);
        }

        Assert.Equal([(A, a), (C, c)], steps);
    }

    private static void AssertLetterOrder<TLayout>(AlphabetTrieNode<EnglishLowercase, TLayout, int> node)
        where TLayout : struct, ITrieLayout
    {
        var c = node.GetOrAddChild(C);
        var a = node.GetOrAddChild(A);
        var children = new AlphabetTrieChildren<EnglishLowercase, TLayout, int>(node);

        Assert.Equal(2, children.Count);
        Assert.Same(a, children.Get(0));
        Assert.Same(c, children.Get(1));
    }
}
