using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class TrieChildrenTests
{
    private const int C = 2;
    private const int A = 0;

    [Fact]
    public void Count_EmptyNode_IsZero()
    {
        var dense = new TrieChildren<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>(new());
        var packed = new TrieChildren<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>(new());

        Assert.Equal(0, dense.Count);
        Assert.Equal(0, packed.Count);
    }

    [Fact]
    public void Get_ListsChildrenInLetterOrder_NotInsertionOrder()
    {
        AssertLetterOrder(new TrieNode<AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>, int>());
        AssertLetterOrder(new TrieNode<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>());
    }

    private static void AssertLetterOrder<TLayout>(TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int> node)
        where TLayout : struct, ITrieLayout
    {
        var c = node.Children.GetOrAddChild(C);
        var a = node.Children.GetOrAddChild(A);
        var children = new TrieChildren<AlphabetSlots<EnglishLowercase, TLayout, int>, int>(node);

        Assert.Equal(2, children.Count);
        Assert.Same(a, children.Get(0));
        Assert.Same(c, children.Get(1));
    }
}
