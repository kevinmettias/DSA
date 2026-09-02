using DSAExperimentation.DataStructures.KeyedDisjointSet;

namespace DSAExperimentation.Tests.DataStructures.KeyedDisjointSet;

public sealed partial class KeyedDisjointSetTests
{
    [Fact]
    public void TryFind_KeyPresent_ReturnsItsOwnRepresentativeInitially()
    {
        var set = new KeyedDisjointSet<string>(["a", "b", "c"]);

        var found = set.TryFind("a", out var representative);

        Assert.True(found);
        Assert.Equal("a", representative);
    }

    [Fact]
    public void TryFind_UnknownKey_ReturnsFalse()
    {
        var set = new KeyedDisjointSet<string>(["a", "b"]);

        var found = set.TryFind("z", out _);

        Assert.False(found);
    }

    [Fact]
    public void TryUnion_TwoPresentKeys_ConnectsThem()
    {
        var set = new KeyedDisjointSet<string>(["a", "b", "c"]);

        var unioned = set.TryUnion("a", "b");
        var connected = set.IsConnected("a", "b");

        Assert.True(unioned);
        Assert.True(connected);
    }

    [Fact]
    public void TryFind_AfterUnion_ReturnsSameRepresentativeForBothKeys()
    {
        var set = new KeyedDisjointSet<string>(["a", "b", "c"]);
        set.TryUnion("a", "b");

        set.TryFind("a", out var aRoot);
        set.TryFind("b", out var bRoot);

        Assert.Equal(aRoot, bRoot);
    }

    [Fact]
    public void TryUnion_UnknownKey_ReturnsFalseAndLeavesOthersUnchanged()
    {
        var set = new KeyedDisjointSet<string>(["a", "b"]);

        var unioned = set.TryUnion("a", "z");
        var stillDisconnected = set.IsConnected("a", "b");

        Assert.False(unioned);
        Assert.False(stillDisconnected);
    }

    [Fact]
    public void IsConnected_UnknownKey_ReturnsFalse()
    {
        var set = new KeyedDisjointSet<string>(["a", "b"]);

        var connected = set.IsConnected("a", "z");

        Assert.False(connected);
    }

    [Fact]
    public void Construction_DuplicateKeys_DedupesToOneId()
    {
        var set = new KeyedDisjointSet<string>(["a", "b", "a", "b", "c"]);

        Assert.Equal(3, set.Count);
    }

    [Fact]
    public void Construction_WithCustomComparer_TreatsKeysAsEqualPerComparer()
    {
        var set = new KeyedDisjointSet<string>(["Key"], StringComparer.OrdinalIgnoreCase);

        var found = set.TryFind("KEY", out var representative);

        Assert.True(found);
        Assert.Equal("Key", representative);
    }

    [Fact]
    public void Union_ChainOfUnions_AllMembersReportConnected()
    {
        var set = new KeyedDisjointSet<string>(["a", "b", "c", "d"]);

        set.TryUnion("a", "b");
        set.TryUnion("b", "c");
        set.TryUnion("c", "d");
        var connected = set.IsConnected("a", "d");

        Assert.True(connected);
    }

    [Fact]
    public void Count_ReportsTheNumberOfDistinctKeysSeeded()
    {
        Assert.Equal(4, new KeyedDisjointSet<string>(["a", "b", "c", "d"]).Count);
    }

    [Fact]
    public void Count_IsUnchangedByUnions()
    {
        // Union merges components, it never removes a key from the index.
        var set = new KeyedDisjointSet<string>(["a", "b", "c"]);

        set.TryUnion("a", "b");

        Assert.Equal(3, set.Count);
    }

    [Fact]
    public void Count_NoKeys_IsZero()
    {
        Assert.Equal(0, new KeyedDisjointSet<string>([]).Count);
    }

    [Fact]
    public void HasKey_SeededKey_ReturnsTrue()
    {
        Assert.True(new KeyedDisjointSet<string>(["a", "b"]).HasKey("a"));
    }

    [Fact]
    public void HasKey_UnknownKey_ReturnsFalse()
    {
        Assert.False(new KeyedDisjointSet<string>(["a", "b"]).HasKey("z"));
    }

    [Fact]
    public void HasKey_HonoursTheSuppliedComparer()
    {
        var set = new KeyedDisjointSet<string>(["Key"], StringComparer.OrdinalIgnoreCase);

        Assert.True(set.HasKey("KEY"));
    }
}
