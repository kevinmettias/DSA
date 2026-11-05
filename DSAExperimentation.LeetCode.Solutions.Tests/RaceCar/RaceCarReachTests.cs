using DSAExperimentation.LeetCode.Solutions.RaceCar;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RaceCar;

public sealed partial class RaceCarReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(RaceCarReach.IsWithin(250));

    [Fact]
    public void IsWithin_TargetPastTheReach_IsFalse() => Assert.False(RaceCarReach.IsWithin(251));
}
