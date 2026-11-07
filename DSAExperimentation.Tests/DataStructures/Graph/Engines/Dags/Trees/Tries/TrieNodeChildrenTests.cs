using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class TrieNodeChildrenTests
{
    private const int Zero = 0;
    private const int One = 1;

    [Fact]
    public void Count_NewNode_IsZero() =>
        Assert.Equal(0, new TrieNodeChildren<BitTrieNode, BinarySlots<BitTrieNode>>(new BitTrieNode()).Count);

    [Fact]
    public void Get_ListsChildrenZeroBeforeOne_NotInInsertionOrder()
    {
        var node = new BitTrieNode();
        var one = node.Children.GetOrAddChildFor(One);
        var zero = node.Children.GetOrAddChildFor(Zero);
        var children = new TrieNodeChildren<BitTrieNode, BinarySlots<BitTrieNode>>(node);

        Assert.Equal(2, children.Count);
        Assert.Same(zero, children.Get(0));
        Assert.Same(one, children.Get(1));
    }
}
