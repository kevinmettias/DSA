using DSAExperimentation.LeetCode.Solutions.New21Game;

namespace DSAExperimentation.LeetCode.Solutions.Tests.New21Game;

public sealed partial class New21GameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(New21GameReach.IsWithin(1, 20, 5));

    [Fact]
    public void IsWithin_StopAtPastTheReach_IsFalse() => Assert.False(New21GameReach.IsWithin(1, 21, 5));

    [Fact]
    public void IsWithin_MaximumPointsPastTheReach_IsFalse() => Assert.False(New21GameReach.IsWithin(1, 20, 6));
}
