using DSAExperimentation.LeetCode.Solutions.LRUCache;
using DSAExperimentation.Mechanisms.Cache.LruCache;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LRUCache;

public sealed partial class LRUCacheCallsTests
{
    [Fact]
    public void Get_KeyTheCacheHolds_IsItsValue()
    {
        var cache = new LruCache<int, int>(2);
        cache.Set(1, 7);

        Assert.Equal(7, LRUCacheCalls.Get(cache, 1));
    }

    [Fact]
    public void Get_KeyTheCacheDoesNotHold_IsMinusOne() => Assert.Equal(-1, LRUCacheCalls.Get(new LruCache<int, int>(2), 1));

    [Fact]
    public void Put_AnyKey_StoresItsValue()
    {
        var cache = new LruCache<int, int>(2);

        LRUCacheCalls.Put(cache, 1, 7);

        Assert.True(cache.TryGetValue(1, out var value) && value == 7);
    }
}
