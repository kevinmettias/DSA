using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class BitTrieTopologyTests
{
    [Fact]
    public void GetChildren_ExposesBothBranchesZeroFirst()
    {
        var zero = new BitTrieNode();
        var one = new BitTrieNode();

        var children = BitTrieTopology.GetChildren(new BitTrieNode { Zero = zero, One = one });

        Assert.Equal(2, children.Count);
        Assert.Same(zero, children.Get(0));
        Assert.Same(one, children.Get(1));
    }

    [Fact]
    public void GetChildren_LeafNode_ReturnsNone() => Assert.Equal(0, BitTrieTopology.GetChildren(new BitTrieNode()).Count);
}
