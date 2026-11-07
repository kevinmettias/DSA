using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A bit trie's node is its children and nothing else, so its one member is the store, handed out by
// reference: a child added through it is the node's own.
public sealed partial class BitTrieNodeTests
{
    private const int One = 1;

    [Fact]
    public void Children_AddThroughTheReference_ChangesTheNodesOwnStore()
    {
        var node = new BitTrieNode();
        var child = node.Children.GetOrAddChildFor(One);

        Assert.Same(child, node.Children.ChildFor(One));
    }

    [Fact]
    public void Children_NewNode_HasNone() => Assert.Equal(0, new BitTrieNode().Children.Count);
}
