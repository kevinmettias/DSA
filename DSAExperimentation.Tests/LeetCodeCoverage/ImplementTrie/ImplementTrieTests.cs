using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementTrie;

// LeetCode 208. Implement Trie (Prefix Tree): Set/HasKey/HasPrefix on this repo's
// existing Trie<TValue> already implement Insert/Search/StartsWith exactly as
// specified - no new code needed at all.
public sealed partial class ImplementTrieTests
{
    [Fact]
    public void InsertSearchStartsWith_ClassicExample_MatchesExpectedBehavior()
    {
        var trie = new Trie<bool>();

        trie.Set("apple", true);

        Assert.True(trie.HasKey("apple"));
        Assert.False(trie.HasKey("app"));
        Assert.True(trie.HasPrefix("app"));

        trie.Set("app", true);

        Assert.True(trie.HasKey("app"));
    }
}
