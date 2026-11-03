using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class AlphabetTrieTopologyTests
{
    private const int Z = 25;
    private const int B = 1;

    // A dense node allocates a slot for every letter; only the filled ones are children.
    [Fact]
    public void GetChildren_EmptyNode_ReturnsNone()
    {
        var dense = AlphabetTrieTopology<EnglishLowercase, DenseTrieLayout, int>.GetChildren(new());
        var packed = AlphabetTrieTopology<EnglishLowercase, PackedTrieLayout, int>.GetChildren(new());

        Assert.Equal(0, dense.Count);
        Assert.Equal(0, packed.Count);
    }

    [Fact]
    public void GetChildren_ListsTheNodesChildrenInLetterOrder()
    {
        var dense = new AlphabetTrieNode<EnglishLowercase, DenseTrieLayout, int>();
        var packed = new AlphabetTrieNode<EnglishLowercase, PackedTrieLayout, int>();
        var denseZ = dense.GetOrAddChild(Z);
        var denseB = dense.GetOrAddChild(B);
        var packedZ = packed.GetOrAddChild(Z);
        var packedB = packed.GetOrAddChild(B);

        var denseChildren = AlphabetTrieTopology<EnglishLowercase, DenseTrieLayout, int>.GetChildren(dense);
        var packedChildren = AlphabetTrieTopology<EnglishLowercase, PackedTrieLayout, int>.GetChildren(packed);

        Assert.Same(denseB, denseChildren.Get(0));
        Assert.Same(denseZ, denseChildren.Get(1));
        Assert.Same(packedB, packedChildren.Get(0));
        Assert.Same(packedZ, packedChildren.Get(1));
    }
}
