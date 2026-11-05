using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Surfaces.Gluings;

// One pair of edges, each way it can be joined: left open, a coordinate past an edge falls off; glued
// straight, it comes back in at the other edge, as many times round as the step is long.
public sealed partial class EdgeGluingTests
{
    private const int Extent = 5;

    [Fact]
    public void Unglued_TryWrap_OnTheBoard_LandsUncrossed()
    {
        var coordinate = Extent - 1;

        Assert.True(Unglued.TryWrap(ref coordinate, Extent, out var crossings));
        Assert.Equal((Extent - 1, 0), (coordinate, crossings));
    }

    [Fact]
    public void Unglued_TryWrap_PastEitherEdge_FallsOff()
    {
        var below = -1;
        var beyond = Extent;

        Assert.False(Unglued.TryWrap(ref below, Extent, out _));
        Assert.False(Unglued.TryWrap(ref beyond, Extent, out _));
    }

    [Fact]
    public void Straight_TryWrap_PastTheLastEdge_ComesInAtTheFirst()
    {
        var coordinate = Extent;

        Assert.True(Straight.TryWrap(ref coordinate, Extent, out var crossings));
        Assert.Equal((0, 1), (coordinate, crossings));
    }

    [Fact]
    public void Straight_TryWrap_BeforeTheFirstEdge_ComesInAtTheLast()
    {
        var coordinate = -1;

        Assert.True(Straight.TryWrap(ref coordinate, Extent, out var crossings));
        Assert.Equal((Extent - 1, -1), (coordinate, crossings));
    }

    // A knight's two-cell jump on a board one or two cells across goes round more than once.
    [Fact]
    public void Straight_TryWrap_AStepLongerThanTheBoard_CrossesOnceForEachWayRound()
    {
        var forward = 5;
        var backward = -3;

        Assert.True(Straight.TryWrap(ref forward, 2, out var forwardCrossings));
        Assert.True(Straight.TryWrap(ref backward, 2, out var backwardCrossings));
        Assert.Equal((1, 2), (forward, forwardCrossings));
        Assert.Equal((1, -2), (backward, backwardCrossings));
    }

    [Fact]
    public void Twisted_TryWrap_WrapsAsAStraightGluingDoes()
    {
        var twisted = -1;
        var straight = -1;

        Assert.True(Twisted.TryWrap(ref twisted, Extent, out var twistedCrossings));
        Assert.True(Straight.TryWrap(ref straight, Extent, out var straightCrossings));
        Assert.Equal((straight, straightCrossings), (twisted, twistedCrossings));
    }

    [Fact]
    public void Twisted_IsTwisted_AndNeitherOtherGluingIs() =>
        Assert.Equal((true, false, false), (Twisted.IsTwisted, Straight.IsTwisted, Unglued.IsTwisted));

    [Fact]
    public void Straight_TryWrap_OnTheBoard_StaysUncrossed()
    {
        var coordinate = 2;

        Assert.True(Straight.TryWrap(ref coordinate, Extent, out var crossings));
        Assert.Equal((2, 0), (coordinate, crossings));
    }
}
