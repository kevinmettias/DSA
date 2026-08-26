using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.DataStructures.HashMap;

public sealed partial class HashMapTests
{
    [Fact]
    public void Set_ThenGet_ReturnsStoredValue()
    {
        var map = new HashMap<string, int>();

        map.Set("a", 1);

        Assert.Equal(1, map.Get("a"));
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue()
    {
        var map = new HashMap<string, int>();
        map.Set("a", 1);

        map.Set("a", 2);

        Assert.Equal(2, map.Get("a"));
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
    public void Get_MissingKey_ThrowsInvalidOperationException()
    {
        var map = new HashMap<string, int>();

        Assert.Throws<InvalidOperationException>(() => map.Get("missing"));
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

        Assert.False(map.HasKey("a"));
        Assert.Equal(2, map.Get("b"));
        Assert.Equal(3, map.Get("c"));
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
            Assert.Equal(i * 2, map.Get(i));
        }
    }

    [Fact]
    public void Set_WithCustomComparer_TreatsKeysAsEqualPerComparer()
    {
        var map = new HashMap<string, int>(StringComparer.OrdinalIgnoreCase);

        map.Set("Key", 1);

        Assert.True(map.HasKey("key"));
        Assert.Equal(1, map.Get("KEY"));
    }
}
