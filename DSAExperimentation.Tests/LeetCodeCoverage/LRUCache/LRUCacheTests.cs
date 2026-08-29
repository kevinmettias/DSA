using DSAExperimentation.DataStructures.Cache.LruCache;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LRUCache;

public sealed partial class LRUCacheTests
{
    [Fact]
    public void LruCache_LeetCodeExample_EvictsLeastRecentlyUsedKey()
    {
        var cache = new LruCache<int, int>(2);
        cache.Set(1, 1); cache.Set(2, 2);
        Assert.True(cache.TryGetValue(1, out var one)); Assert.Equal(1, one);
        cache.Set(3, 3);
        Assert.False(cache.TryGetValue(2, out _));
        cache.Set(4, 4);
        Assert.False(cache.TryGetValue(1, out _));
        Assert.True(cache.TryGetValue(3, out var three)); Assert.Equal(3, three);
        Assert.True(cache.TryGetValue(4, out var four)); Assert.Equal(4, four);
    }
}
