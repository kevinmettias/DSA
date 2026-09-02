using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.DataStructures.DisjointSet;

public sealed partial class DisjointSetForestTests
{
    [Fact]
    public void Constructor_EachIdStartsAsItsOwnParentWithZeroRank()
    {
        var forest = new DisjointSetForest(3);

        Assert.Equal(0, forest.GetParent(0));
        Assert.Equal(1, forest.GetParent(1));
        Assert.Equal(2, forest.GetParent(2));
        Assert.Equal(0, forest.GetRank(0));
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var forest = new DisjointSetForest(5);

        Assert.Equal(5, forest.Count);
    }

    [Fact]
    public void SetParent_OverwritesParentAtId()
    {
        var forest = new DisjointSetForest(3);

        forest.SetParent(0, 1);

        Assert.Equal(1, forest.GetParent(0));
    }

    [Fact]
    public void IncrementRank_IncreasesRankAtId()
    {
        var forest = new DisjointSetForest(3);

        forest.IncrementRank(0);
        forest.IncrementRank(0);

        Assert.Equal(2, forest.GetRank(0));
    }

    [Fact]
    public void GetParent_IdOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var forest = new DisjointSetForest(3);

        Assert.Throws<ArgumentOutOfRangeException>(() => forest.GetParent(3));
    }

    [Fact]
    public void GetParent_NegativeId_ThrowsArgumentOutOfRangeException()
    {
        var forest = new DisjointSetForest(3);

        Assert.Throws<ArgumentOutOfRangeException>(() => forest.GetParent(-1));
    }

    [Fact]
    public void TryGetParent_ValidId_ReturnsTrueAndParent()
    {
        var forest = new DisjointSetForest(3);

        var found = forest.TryGetParent(1, out var parent);

        Assert.True(found);
        Assert.Equal(1, parent);
    }

    [Fact]
    public void TryGetParent_IdOutOfRange_ReturnsFalse()
    {
        var forest = new DisjointSetForest(3);

        var found = forest.TryGetParent(3, out _);

        Assert.False(found);
    }

    [Fact]
    public void TrySetParent_ValidId_ReturnsTrueAndOverwritesParent()
    {
        var forest = new DisjointSetForest(3);

        var succeeded = forest.TrySetParent(0, 1);

        Assert.True(succeeded);
        Assert.Equal(1, forest.GetParent(0));
    }

    [Fact]
    public void TrySetParent_IdOutOfRange_ReturnsFalse()
    {
        var forest = new DisjointSetForest(3);

        var succeeded = forest.TrySetParent(3, 1);

        Assert.False(succeeded);
    }

    [Fact]
    public void TryGetRank_ValidId_ReturnsTrueAndRank()
    {
        var forest = new DisjointSetForest(3);
        forest.IncrementRank(0);

        var found = forest.TryGetRank(0, out var rank);

        Assert.True(found);
        Assert.Equal(1, rank);
    }

    [Fact]
    public void TryGetRank_IdOutOfRange_ReturnsFalse()
    {
        var forest = new DisjointSetForest(3);

        var found = forest.TryGetRank(3, out _);

        Assert.False(found);
    }

    [Fact]
    public void TryIncrementRank_ValidId_ReturnsTrueAndIncreasesRank()
    {
        var forest = new DisjointSetForest(3);

        var succeeded = forest.TryIncrementRank(0);

        Assert.True(succeeded);
        Assert.Equal(1, forest.GetRank(0));
    }

    [Fact]
    public void TryIncrementRank_IdOutOfRange_ReturnsFalse()
    {
        var forest = new DisjointSetForest(3);

        var succeeded = forest.TryIncrementRank(3);

        Assert.False(succeeded);
    }

    [Fact]
    public void GetRank_FreshElement_StartsAtZero()
    {
        Assert.Equal(0, new DisjointSetForest(4).GetRank(0));
    }

    [Fact]
    public void GetRank_ReflectsEveryIncrement()
    {
        var forest = new DisjointSetForest(4);

        forest.IncrementRank(2);
        forest.IncrementRank(2);

        Assert.Equal(2, forest.GetRank(2));
    }

    [Fact]
    public void GetRank_IsPerElement()
    {
        var forest = new DisjointSetForest(4);

        forest.IncrementRank(1);

        Assert.Equal(1, forest.GetRank(1));
        Assert.Equal(0, forest.GetRank(0));
    }
}
