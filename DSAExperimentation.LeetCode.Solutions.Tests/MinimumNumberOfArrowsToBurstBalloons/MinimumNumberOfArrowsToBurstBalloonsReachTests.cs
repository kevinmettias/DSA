using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfArrowsToBurstBalloons;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfArrowsToBurstBalloons;

public sealed partial class MinimumNumberOfArrowsToBurstBalloonsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumNumberOfArrowsToBurstBalloonsReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 9).ToArray()));

    [Fact]
    public void IsWithin_PointsLengthPastTheReach_IsFalse() => Assert.False(MinimumNumberOfArrowsToBurstBalloonsReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 10).ToArray()));
}
