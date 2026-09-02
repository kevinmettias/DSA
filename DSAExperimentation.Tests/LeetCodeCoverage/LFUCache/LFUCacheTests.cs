using DSAExperimentation.DataStructures.Cache.LfuCache;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LFUCache;

public sealed partial class LFUCacheTests
{
    [Fact]
    public void LfuCache_Example_EvictsLeastFrequentlyUsed()
    {
        var cache = new LfuCache<int, int>(2);
        cache.Set(1, 1);
        cache.Set(2, 2);

        var foundOne = cache.TryGetValue(1, out var one);

        Assert.True(foundOne);
        Assert.Equal(1, one);

        cache.Set(3, 3);

        var foundTwo = cache.TryGetValue(2, out _);

        Assert.False(foundTwo);

        var foundThree = cache.TryGetValue(3, out var three);

        Assert.True(foundThree);
        Assert.Equal(3, three);
    }
}
