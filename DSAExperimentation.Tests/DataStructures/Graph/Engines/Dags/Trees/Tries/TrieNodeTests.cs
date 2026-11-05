using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A node's own state: whether a key ends on it. Its children are its store's, tested in AlphabetSlotsTests.
public sealed partial class TrieNodeTests
{
    [Fact]
    public void HasValue_NewNode_IsFalse() =>
        Assert.False(new TrieNode<AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>, int>().HasValue);
}
