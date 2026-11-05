using DSAExperimentation.LeetCode.Solutions.LFUCache;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LFUCache;

public sealed partial class LFUCacheCallsTests
{
    [Fact]
    public void Get_KeyTheCacheHolds_IsItsValue()
    {
        var cache = LFUCacheSolution.CreateByLfuCachePrimitive(2);
        cache.Set(1, 7);

        Assert.Equal(7, LFUCacheCalls.Get(cache, 1));
    }

    [Fact]
    public void Get_KeyTheCacheDoesNotHold_IsMinusOne() => Assert.Equal(-1, LFUCacheCalls.Get(LFUCacheSolution.CreateByLfuCachePrimitive(2), 1));

    [Fact]
    public void Put_AnyKey_StoresItsValue()
    {
        var cache = LFUCacheSolution.CreateByLfuCachePrimitive(2);

        LFUCacheCalls.Put(cache, 1, 7);

        Assert.True(cache.TryGetValue(1, out var value) && value == 7);
    }
}
