using DSAExperimentation.LeetCode.Solutions.MinimumCostTreeFromLeafValues;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostTreeFromLeafValues;

public sealed partial class MinimumCostTreeFromLeafValuesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumCostTreeFromLeafValuesReach.IsWithin(new int[14]));

    [Fact]
    public void IsWithin_ArrLengthPastTheReach_IsFalse() => Assert.False(MinimumCostTreeFromLeafValuesReach.IsWithin(new int[15]));
}
