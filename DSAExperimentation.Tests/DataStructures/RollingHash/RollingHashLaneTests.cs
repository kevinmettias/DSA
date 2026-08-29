using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Tests.DataStructures.RollingHash;

public sealed partial class RollingHashLaneTests
{
    [Fact]
    public void DefaultFirst_HasDocumentedBaseAndModulus()
    {
        var lane = RollingHashLane.DefaultFirst;

        Assert.Equal(131, lane.Base);
        Assert.Equal(1_000_000_007, lane.Modulus);
    }

    [Fact]
    public void DefaultSecond_HasDocumentedBaseAndModulus()
    {
        var lane = RollingHashLane.DefaultSecond;

        Assert.Equal(137, lane.Base);
        Assert.Equal(998_244_353, lane.Modulus);
    }
}
