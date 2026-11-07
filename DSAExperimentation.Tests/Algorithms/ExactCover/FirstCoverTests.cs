using DSAExperimentation.Algorithms.ExactCover;

namespace DSAExperimentation.Tests.Algorithms.ExactCover;

public sealed partial class FirstCoverTests
{
    [Fact]
    public void Rows_NoCoverSeen_IsEmpty() => Assert.Empty(new FirstCover().Rows);

    [Fact]
    public void ShouldStopAfter_ACover_StopsAndKeepsACopy()
    {
        var visitor = new FirstCover();
        int[] buffer = [4, 1, 7];

        var shouldStop = visitor.ShouldStopAfter(buffer);
        buffer[0] = 0;

        Assert.True(shouldStop);
        Assert.Equal([4, 1, 7], visitor.Rows);
    }
}
