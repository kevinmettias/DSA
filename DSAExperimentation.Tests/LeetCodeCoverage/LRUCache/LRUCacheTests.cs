using DSAExperimentation.DataStructures.Cache.LruCache;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LRUCache;

public sealed partial class LRUCacheTests
{
    [Fact]
    public void LruCache_LeetCodeExample_EvictsLeastRecentlyUsedKey()
    {
        var cache = new LruCache<int, int>(2);
        cache.Set(1, 1); cache.Set(2, 2);
        var containsOne = cache.TryGetValue(1, out var one);
        Assert.True(containsOne); Assert.Equal(1, one);
        cache.Set(3, 3);
        var containsTwo = cache.TryGetValue(2, out _);
        Assert.False(containsTwo);
        cache.Set(4, 4);
        var containsOneAfterEviction = cache.TryGetValue(1, out _);
        Assert.False(containsOneAfterEviction);
        var containsThree = cache.TryGetValue(3, out var three);
        Assert.True(containsThree); Assert.Equal(3, three);
        var containsFour = cache.TryGetValue(4, out var four);
        Assert.True(containsFour); Assert.Equal(4, four);
    }
}
