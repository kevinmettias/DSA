using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.DataStructures.DisjointSet;

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
        set.TryFind("a", out var aRoot);
        set.TryFind("b", out var bRoot);

        Assert.True(unioned);
        Assert.True(connected);
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
}
