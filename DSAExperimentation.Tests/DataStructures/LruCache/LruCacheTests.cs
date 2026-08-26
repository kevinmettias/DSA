using DSAExperimentation.DataStructures.LruCache;

namespace DSAExperimentation.Tests.DataStructures.LruCache;

public sealed partial class LruCacheTests
{
    [Fact]
    public void Count_ReflectsNumberOfStoredKeys()
    {
        var cache = new LruCache<string, int>(2);

        cache.Set("a", 1);
        cache.Set("b", 2);

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Constructor_CapacityLessThanOne_ThrowsArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<string, int>(0));

    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var cache = new LruCache<string, int>(2);

        cache.Set("a", 1);
        var found = cache.TryGetValue("a", out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1);

        cache.Set("a", 2);
        cache.TryGetValue("a", out var value);

        Assert.Equal(2, value);
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        var cache = new LruCache<string, int>(2);

        var found = cache.TryGetValue("missing", out _);

        Assert.False(found);
    }

    [Fact]
    public void Set_BeyondCapacity_EvictsLeastRecentlyUsedKey()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        cache.Set("c", 3);

        var foundA = cache.TryGetValue("a", out _);
        var foundB = cache.TryGetValue("b", out _);
        var foundC = cache.TryGetValue("c", out _);

        Assert.False(foundA);
        Assert.True(foundB);
        Assert.True(foundC);
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void TryGetValue_PromotesKeyAndProtectsItFromEviction()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        cache.TryGetValue("a", out _);
        cache.Set("c", 3);

        var foundA = cache.TryGetValue("a", out _);
        var foundB = cache.TryGetValue("b", out _);

        Assert.True(foundA);
        Assert.False(foundB);
    }

    [Fact]
    public void Set_ExistingKey_PromotesKeyAndProtectsItFromEviction()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        cache.Set("a", 10);
        cache.Set("c", 3);

        var foundA = cache.TryGetValue("a", out var value);
        var foundB = cache.TryGetValue("b", out _);

        Assert.True(foundA);
        Assert.Equal(10, value);
        Assert.False(foundB);
    }
}
