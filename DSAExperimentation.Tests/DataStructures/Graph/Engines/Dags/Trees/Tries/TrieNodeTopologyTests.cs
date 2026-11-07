using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The witness is real, not just satisfied: TreeMetrics over it counts a bit trie's nodes in
// BitTrieTests; here it lists a node's children through GetChildren.
public sealed partial class TrieNodeTopologyTests
{
    private const int One = 1;

    [Fact]
    public void GetChildren_ListsTheNodesChildren()
    {
        var node = new BitTrieNode();
        var one = node.Children.GetOrAddChildFor(One);

        var children = TrieNodeTopology<BitTrieNode, BinarySlots<BitTrieNode>>.GetChildren(node);

        Assert.Equal(1, children.Count);
        Assert.Same(one, children.Get(0));
    }
}
