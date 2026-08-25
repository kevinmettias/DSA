using DSAExperimentation.DataStructures.Collections.DisjointSet;

namespace DSAExperimentation.Tests.DataStructures.Collections.DisjointSet;

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
}
