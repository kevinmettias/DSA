using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.DataStructures.HashMap;

public sealed partial class HashMapTests
{
    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var map = new HashMap<string, int>();

        map.Set("a", 1);
        var found = map.TryGetValue("a", out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);

        map.Set("a", 2);
        map.TryGetValue("a", out var value);

        Assert.Equal(2, value);
        Assert.Equal(1, map.Count);
    }

    [Fact]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        var map = new HashMap<string, int>();

        var found = map.TryGetValue("missing", out _);

        Assert.False(found);
    }

    [Fact]
    public void HasKey_ReflectsPresenceOfKey()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);

        Assert.True(map.HasKey("a"));
        Assert.False(map.HasKey("b"));
    }

    [Fact]
    public void TryRemove_ExistingKey_RemovesItAndReturnsTrue()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);

        Assert.True(map.TryRemove("a"));
        Assert.False(map.HasKey("a"));
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void TryRemove_MissingKey_ReturnsFalse()
    {
        var map = new HashMap<string, int>();

        Assert.False(map.TryRemove("missing"));
    }

    [Fact]
    public void TryRemove_ThenSetNewKey_ReusesFreedSlotAndStaysCorrect()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);
        map.Set("b", 2);

        map.TryRemove("a");
        map.Set("c", 3);
        map.TryGetValue("b", out var valueAtB);
        map.TryGetValue("c", out var valueAtC);

        Assert.False(map.HasKey("a"));
        Assert.Equal(2, valueAtB);
        Assert.Equal(3, valueAtC);
        Assert.Equal(2, map.Count);
    }

    [Fact]
    public void Set_BeyondLoadFactorThreshold_GrowsAndRetainsAllEntries()
    {
        var map = new HashMap<int, int>();

        for (var i = 0; i < 100; i++)
        {
            map.Set(i, i * 2);
        }

        Assert.Equal(100, map.Count);

        for (var i = 0; i < 100; i++)
        {
            map.TryGetValue(i, out var value);

            Assert.Equal(i * 2, value);
        }
    }

    [Fact]
    public void Set_WithCustomComparer_TreatsKeysAsEqualPerComparer()
    {
        var map = new HashMap<string, int>(StringComparer.OrdinalIgnoreCase);

        map.Set("Key", 1);
        map.TryGetValue("KEY", out var value);

        Assert.True(map.HasKey("key"));
        Assert.Equal(1, value);
    }

    [Fact]
    public void Keys_AfterSet_ReturnsAllKeys()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);
        map.Set("b", 2);

        var keys = map.Keys;

        Assert.Equal(["a", "b"], keys.OrderBy(key => key));
    }

    [Fact]
    public void Values_AfterSet_ReturnsAllValues()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);
        map.Set("b", 2);

        var values = map.Values;

        Assert.Equal([1, 2], values.OrderBy(value => value));
    }

    [Fact]
    public void Keys_AfterGrow_RetainsAllKeys()
    {
        var map = new HashMap<int, int>();

        for (var i = 0; i < 100; i++)
        {
            map.Set(i, i * 2);
        }

        var keys = map.Keys;
        var expectedKeys = Enumerable.Range(0, 100);

        Assert.Equal(expectedKeys, keys.OrderBy(key => key));
    }

    [Fact]
    public void Keys_AfterTryRemove_ExcludesRemovedKey()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);
        map.Set("b", 2);

        map.TryRemove("a");
        var keys = map.Keys;

        Assert.Equal(["b"], keys);
    }
}
