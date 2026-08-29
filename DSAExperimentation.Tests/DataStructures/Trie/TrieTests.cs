using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.DataStructures.Trie;

public sealed partial class TrieTests
{
    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var trie = new Trie<int>();

        trie.Set("cat", 1);
        var found = trie.TryGetValue("cat", out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue_CountUnchanged()
    {
        var trie = new Trie<int>();
        trie.Set("cat", 1);

        trie.Set("cat", 2);
        trie.TryGetValue("cat", out var value);

        Assert.Equal(2, value);
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        var trie = new Trie<int>();
        trie.Set("cat", 1);

        Assert.True(trie.HasKey("cat"));
        Assert.False(trie.HasKey("dog"));
    }

    [Fact]
    public void TryGetValue_UnknownKey_ReturnsFalseAndDefault()
    {
        var trie = new Trie<int>();

        var found = trie.TryGetValue("missing", out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void HasPrefix_KnownPrefix_ReturnsTrue()
    {
        var trie = new Trie<int>();
        trie.Set("card", 1);

        Assert.True(trie.HasPrefix("car"));
        Assert.True(trie.HasPrefix("card"));
    }

    [Fact]
    public void HasPrefix_UnknownPrefix_ReturnsFalse()
    {
        var trie = new Trie<int>();
        trie.Set("card", 1);

        Assert.False(trie.HasPrefix("dog"));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsTrueWhenAnyKeyExists()
    {
        var trie = new Trie<int>();
        trie.Set("card", 1);

        Assert.True(trie.HasPrefix(""));
    }

    [Fact]
    public void HasPrefix_EmptyPrefix_ReturnsFalseWhenEmpty()
    {
        var trie = new Trie<int>();

        Assert.False(trie.HasPrefix(""));
    }

    [Fact]
    public void SharedPrefixNode_TracksEachWordIndependently()
    {
        // "car" is a prefix of "card" - both must be independently retrievable, and
        // "car" must count as present via HasKey even though a longer word continues
        // past it in the same shared node chain.
        var trie = new Trie<int>();
        trie.Set("card", 1);

        Assert.True(trie.HasPrefix("car"));
        Assert.False(trie.HasKey("car"));

        trie.Set("car", 2);

        Assert.True(trie.HasKey("car"));
        Assert.True(trie.HasKey("card"));
        trie.TryGetValue("car", out var carValue);
        trie.TryGetValue("card", out var cardValue);
        Assert.Equal(2, carValue);
        Assert.Equal(1, cardValue);
    }

    [Fact]
    public void Count_TracksDistinctKeysOnly()
    {
        var trie = new Trie<int>();

        trie.Set("cat", 1);
        trie.Set("car", 2);
        trie.Set("card", 3);
        trie.Set("car", 4);

        Assert.Equal(3, trie.Count);
    }
}
