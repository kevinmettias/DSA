using DSAExperimentation.LeetCode.Solutions.MinCostClimbingStairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinCostClimbingStairs;

public sealed partial class MinCostClimbingStairsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinCostClimbingStairsReach.IsWithin(new int[31]));

    [Fact]
    public void IsWithin_CostLengthPastTheReach_IsFalse() => Assert.False(MinCostClimbingStairsReach.IsWithin(new int[32]));
}
