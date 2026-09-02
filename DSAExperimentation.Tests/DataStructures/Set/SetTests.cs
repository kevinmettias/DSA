using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.DataStructures.Set;

public sealed partial class SetTests
{
    [Fact]
    public void TryAdd_NewItem_ReturnsTrueAndIncreasesCount()
    {
        var set = new Set<string>();

        Assert.True(set.TryAdd("a"));
        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void TryAdd_DuplicateItem_ReturnsFalseAndDoesNotIncreaseCount()
    {
        var set = new Set<string>();
        set.TryAdd("a");

        Assert.False(set.TryAdd("a"));
        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Has_ReflectsMembership()
    {
        var set = new Set<string>();
        set.TryAdd("a");

        Assert.True(set.Has("a"));
        Assert.False(set.Has("b"));
    }

    [Fact]
    public void TryRemove_ExistingItem_RemovesItAndReturnsTrue()
    {
        var set = new Set<string>();
        set.TryAdd("a");

        Assert.True(set.TryRemove("a"));
        Assert.False(set.Has("a"));
        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void TryRemove_MissingItem_ReturnsFalse()
    {
        var set = new Set<string>();

        Assert.False(set.TryRemove("missing"));
    }

    [Fact]
    public void Count_NewSet_IsZero()
    {
        Assert.Equal(0, new Set<int>().Count);
    }

    [Fact]
    public void Count_RisesOnlyForItemsNotAlreadyPresent()
    {
        var set = new Set<int>();

        set.TryAdd(1);
        set.TryAdd(2);
        set.TryAdd(1);

        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void Count_FallsWhenAnItemIsRemoved()
    {
        var set = new Set<int>();
        set.TryAdd(1);

        set.TryRemove(1);

        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void Count_BulkSeededSet_ReflectsTheDistinctItems()
    {
        Assert.Equal(3, new Set<int>([1, 2, 3, 2, 1]).Count);
    }
}
