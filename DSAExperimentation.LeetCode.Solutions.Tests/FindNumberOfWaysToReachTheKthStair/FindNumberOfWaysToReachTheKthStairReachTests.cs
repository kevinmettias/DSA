using DSAExperimentation.LeetCode.Solutions.FindNumberOfWaysToReachTheKthStair;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindNumberOfWaysToReachTheKthStair;

public sealed partial class FindNumberOfWaysToReachTheKthStairReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindNumberOfWaysToReachTheKthStairReach.IsWithin(5_000_000));

    [Fact]
    public void IsWithin_TargetStairPastTheReach_IsFalse() => Assert.False(FindNumberOfWaysToReachTheKthStairReach.IsWithin(5_000_001));
}
