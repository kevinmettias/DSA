using DSAExperimentation.DataStructures.Cache.LfuCache;

namespace DSAExperimentation.Tests.DataStructures.Cache.LfuCache;

public sealed partial class LfuCacheTests
{
    [Fact]
    public void Count_ReflectsNumberOfStoredKeys()
    {
        var cache = new LfuCache<int, int>(2);

        cache.Set(1, 1);
        cache.Set(2, 2);

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Constructor_CapacityLessThanOne_ThrowsArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new LfuCache<int, int>(0));

    [Fact]
    public void Set_ThenTryGetValue_ReturnsTrueAndStoredValue()
    {
        var cache = new LfuCache<int, int>(2);

        cache.Set(1, 1);
        var found = cache.TryGetValue(1, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Set_ExistingKey_OverwritesValue()
    {
        var cache = new LfuCache<int, int>(2);
        cache.Set(1, 1);

        cache.Set(1, 10);
        cache.TryGetValue(1, out var value);

        Assert.Equal(10, value);
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        var cache = new LfuCache<int, int>(2);

        var found = cache.TryGetValue(999, out _);

        Assert.False(found);
    }

    [Fact]
    public void Set_BeyondCapacity_EvictsLeastFrequentlyUsedKey()
    {
        var cache = new LfuCache<int, int>(2);
        cache.Set(1, 1);
        cache.Set(2, 2);
        cache.TryGetValue(1, out _); // key 1's frequency is now 2, key 2's is still 1

        cache.Set(3, 3); // evicts key 2, the least frequently used

        var foundTwo = cache.TryGetValue(2, out _);
        var foundOne = cache.TryGetValue(1, out _);
        var foundThree = cache.TryGetValue(3, out _);

        Assert.False(foundTwo);
        Assert.True(foundOne);
        Assert.True(foundThree);
    }

    [Fact]
    public void Set_BeyondCapacity_TiesBrokenByLeastRecentlyUsed()
    {
        // Mirrors LeetCode 460's own worked example.
        var cache = new LfuCache<int, int>(2);
        cache.Set(1, 1);
        cache.Set(2, 2);
        cache.TryGetValue(1, out _); // key 1 -> frequency 2
        cache.Set(3, 3); // evicts key 2 (frequency 1, the sole minimum)
        cache.TryGetValue(2, out _); // miss, already evicted
        cache.TryGetValue(3, out _); // key 3 -> frequency 2, now tied with key 1

        cache.Set(4, 4); // key 1 and key 3 are tied at frequency 2; key 1 was touched
                          // longer ago, so it's the least recently used of the pair

        var foundOne = cache.TryGetValue(1, out _);
        var foundThree = cache.TryGetValue(3, out var valueAtThree);
        var foundFour = cache.TryGetValue(4, out var valueAtFour);

        Assert.False(foundOne);
        Assert.True(foundThree);
        Assert.True(foundFour);
        Assert.Equal(3, valueAtThree);
        Assert.Equal(4, valueAtFour);
    }

    [Fact]
    public void TryGetValue_RepeatedAccess_ProtectsKeyFromEviction()
    {
        var cache = new LfuCache<int, int>(2);
        cache.Set(1, 1);
        cache.Set(2, 2);

        cache.TryGetValue(1, out _);
        cache.TryGetValue(1, out _);
        cache.TryGetValue(1, out _);
        cache.Set(3, 3); // key 2 is still at frequency 1, the minimum - evicted

        var foundOne = cache.TryGetValue(1, out _);
        var foundTwo = cache.TryGetValue(2, out _);
        var foundThree = cache.TryGetValue(3, out _);

        Assert.True(foundOne);
        Assert.False(foundTwo);
        Assert.True(foundThree);
    }
}
