using DSAExperimentation.Algorithms.Set;

namespace DSAExperimentation.Tests.Algorithms.Set;

public sealed partial class SetTests
{
    [Fact]
    public void Add_NewItem_ReturnsTrueAndIncreasesCount()
    {
        var set = new Set<string>();

        Assert.True(set.Add("a"));
        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Add_DuplicateItem_ReturnsFalseAndDoesNotIncreaseCount()
    {
        var set = new Set<string>();
        set.Add("a");

        Assert.False(set.Add("a"));
        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Contains_ReflectsMembership()
    {
        var set = new Set<string>();
        set.Add("a");

        Assert.True(set.Contains("a"));
        Assert.False(set.Contains("b"));
    }

    [Fact]
    public void Remove_ExistingItem_RemovesItAndReturnsTrue()
    {
        var set = new Set<string>();
        set.Add("a");

        Assert.True(set.Remove("a"));
        Assert.False(set.Contains("a"));
        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void Remove_MissingItem_ReturnsFalse()
    {
        var set = new Set<string>();

        Assert.False(set.Remove("missing"));
    }
}
