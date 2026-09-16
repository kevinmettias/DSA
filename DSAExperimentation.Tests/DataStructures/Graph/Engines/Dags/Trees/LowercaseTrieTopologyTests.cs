using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class LowercaseTrieTopologyTests
{
    [Fact]
    public void GetChildren_EmptyNode_ReturnsNoneDespiteTheFullSlotArray()
    {
        // The node always allocates 26 slots; only occupied ones are children.
        var node = new LowercaseTrieNode<int>();

        Assert.Equal(LowercaseAlphabet.Size, node.Children.Length);
        Assert.Equal(0, LowercaseTrieTopology<int>.GetChildren(node).Count);
    }

    [Fact]
    public void GetChildren_CountsOnlyOccupiedSlots()
    {
        var node = new LowercaseTrieNode<int>();
        node.Children['c' - 'a'] = new LowercaseTrieNode<int>();
        node.Children['a' - 'a'] = new LowercaseTrieNode<int>();

        Assert.Equal(2, LowercaseTrieTopology<int>.GetChildren(node).Count);
    }

    [Fact]
    public void GetChildren_YieldsChildrenInAlphabeticalSlotOrder()
    {
        var node = new LowercaseTrieNode<int>();
        var c = new LowercaseTrieNode<int> { Value = 3 };
        var a = new LowercaseTrieNode<int> { Value = 1 };
        node.Children['c' - 'a'] = c;
        node.Children['a' - 'a'] = a;

        var children = LowercaseTrieTopology<int>.GetChildren(node);

        Assert.Same(a, children.Get(0));
        Assert.Same(c, children.Get(1));
    }
}
