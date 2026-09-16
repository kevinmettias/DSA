using DSAExperimentation.LeetCode.ImplementTrie;

namespace DSAExperimentation.LeetCode.Tests.ImplementTrie;

// The seam ImplementTrieSolution crosses in the open: its entry point is not a
// function that consumes DataStructures.Trie, it RETURNS one. CreateByTriePrimitive
// hands LeetCode's Insert/Search/StartsWith surface back as this repo's own
// Trie<bool>, so the caller of the solution is holding a live tree whose nodes are
// shared between every key inserted through it.
//
// That makes every test here a statement about the returned structure, not about
// the solution: adding one key must not make another key's node appear to hold a
// value, and HasPrefix must follow the same nodes HasKey follows without requiring
// a terminal flag. A prefix tree that is right per key but wrong per node passes
// every single-key test and fails these.
public sealed partial class TrieReturnedBySolutionSeamTests
{
    [Fact]
    public void HasKey_KeyThatIsAProperPrefixOfAnother_StaysFalse()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("apple", value: true);

        Assert.True(trie.HasKey("apple"));
        Assert.False(trie.HasKey("app"));
    }

    [Fact]
    public void HasPrefix_WalkedPathOfALongerKey_StaysTrue()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("apple", value: true);

        Assert.True(trie.HasPrefix("app"));
        Assert.True(trie.HasPrefix("apple"));
    }

    // Two keys sharing a spine: inserting the second must reuse the nodes the first
    // created rather than marking them terminal, so the shared prefix still has no
    // key of its own.
    [Fact]
    public void Set_TwoKeysSharingASpine_LeavesTheSharedPathNonTerminal()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("app", value: true);
        trie.Set("apple", value: true);

        Assert.True(trie.HasKey("app"));
        Assert.True(trie.HasKey("apple"));
        Assert.False(trie.HasKey("ap"));
        Assert.Equal(2, trie.Count);
    }

    // Re-inserting an existing key must overwrite its value in place, not add a
    // second entry - the tree counts keys, not insertions.
    [Fact]
    public void Set_ExistingKeyTwice_KeepsOneKeyInTheTree()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("app", value: true);
        trie.Set("app", value: false);

        Assert.Equal(1, trie.Count);
        Assert.True(trie.TryGetValue("app", out var value));
        Assert.False(value);
    }

    // HasPrefix on the empty string asks whether the tree holds anything at all -
    // the boundary where the walk visits no node and the answer comes from the key
    // count instead.
    [Fact]
    public void HasPrefix_EmptyPrefix_ReportsWhetherTheTreeHoldsAnyKey()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();

        Assert.False(trie.HasPrefix(string.Empty));

        trie.Set("a", value: true);

        Assert.True(trie.HasPrefix(string.Empty));
    }

    [Fact]
    public void HasPrefix_PathThatDivergesFromEveryKey_StaysFalse()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("apple", value: true);
        trie.Set("apply", value: true);

        Assert.False(trie.HasPrefix("apricot"));
        Assert.False(trie.HasPrefix("apples"));
        Assert.False(trie.HasKey("applyy"));
        Assert.Equal(2, trie.Count);
    }

    // Keys with no characters in common still share the root, so the tree's own
    // bookkeeping is exercised with two independent spines.
    [Fact]
    public void Set_DisjointKeys_KeepsBothSpinesIndependentlyWalkable()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        trie.Set("a", value: true);
        trie.Set("zebra", value: true);

        Assert.True(trie.HasKey("a"));
        Assert.True(trie.HasKey("zebra"));
        Assert.True(trie.HasPrefix("ze"));
        Assert.False(trie.HasPrefix("b"));
        Assert.Equal(2, trie.Count);
    }
}
