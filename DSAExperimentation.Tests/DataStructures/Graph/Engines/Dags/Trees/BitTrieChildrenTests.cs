using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class BitTrieChildrenTests
{
    [Fact]
    public void Count_NoBranches_IsZero()
    {
        Assert.Equal(0, new BitTrieChildren(new BitTrieNode()).Count);
    }

    [Fact]
    public void Count_OnlyTheOneBranch_IsOne()
    {
        Assert.Equal(1, new BitTrieChildren(new BitTrieNode { One = new BitTrieNode() }).Count);
    }

    [Fact]
    public void Count_BothBranches_IsTwo()
    {
        var node = new BitTrieNode { Zero = new BitTrieNode(), One = new BitTrieNode() };

        Assert.Equal(2, new BitTrieChildren(node).Count);
    }

    [Fact]
    public void Get_BothBranches_YieldsZeroThenOne()
    {
        var zero = new BitTrieNode();
        var one = new BitTrieNode();
        var children = new BitTrieChildren(new BitTrieNode { Zero = zero, One = one });

        Assert.Same(zero, children.Get(0));
        Assert.Same(one, children.Get(1));
    }

    [Fact]
    public void Get_OnlyTheOneBranch_PutsItAtIndexZero()
    {
        var one = new BitTrieNode();
        var children = new BitTrieChildren(new BitTrieNode { One = one });

        Assert.Same(one, children.Get(0));
    }

    [Fact]
    public void Get_PastTheLastPresentBranch_Throws()
    {
        var children = new BitTrieChildren(new BitTrieNode { Zero = new BitTrieNode() });

        Assert.Throws<IndexOutOfRangeException>(() => children.Get(1));
    }
}
