using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A counted bit trie's node is its children and a count. The store is handed out by reference, so a
// child added through it is the node's own, and a new node counts no value.
public sealed partial class CountedBitTrieNodeTests
{
    private const int One = 1;

    [Fact]
    public void Children_AddThroughTheReference_ChangesTheNodesOwnStore()
    {
        var node = new CountedBitTrieNode();
        var child = node.Children.GetOrAddChildFor(One);

        Assert.Same(child, node.Children.ChildFor(One));
    }

    [Fact]
    public void Count_NewNode_IsZero() => Assert.Equal(0, new CountedBitTrieNode().Count);
}
