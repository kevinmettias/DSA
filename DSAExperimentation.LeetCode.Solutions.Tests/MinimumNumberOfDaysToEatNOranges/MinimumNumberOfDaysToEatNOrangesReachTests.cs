using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfDaysToEatNOranges;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfDaysToEatNOranges;

public sealed partial class MinimumNumberOfDaysToEatNOrangesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumNumberOfDaysToEatNOrangesReach.IsWithin(666_666_666));

    [Fact]
    public void IsWithin_OrangeCountPastTheReach_IsFalse() => Assert.False(MinimumNumberOfDaysToEatNOrangesReach.IsWithin(666_666_667));
}
