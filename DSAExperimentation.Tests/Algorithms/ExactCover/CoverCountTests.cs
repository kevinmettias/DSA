using DSAExperimentation.Algorithms.ExactCover;

namespace DSAExperimentation.Tests.Algorithms.ExactCover;

public sealed partial class CoverCountTests
{
    [Fact]
    public void Count_NoCoverSeen_IsZero() => Assert.Equal(0, new CoverCount().Count);

    [Fact]
    public void ShouldStopAfter_EachCover_CountsItAndGoesOn()
    {
        var visitor = new CoverCount();

        Assert.False(visitor.ShouldStopAfter([1, 2]));
        Assert.False(visitor.ShouldStopAfter([]));
        Assert.Equal(2, visitor.Count);
    }
}
