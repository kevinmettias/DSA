using DSAExperimentation.LeetCode.Solutions.MinimumXorSumOfTwoArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumXorSumOfTwoArrays;

public sealed partial class MinimumXorSumOfTwoArraysReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumXorSumOfTwoArraysReach.IsWithin(new int[4], new int[0]));

    [Fact]
    public void IsWithin_Nums1LengthPastTheReach_IsFalse() => Assert.False(MinimumXorSumOfTwoArraysReach.IsWithin(new int[5], new int[0]));
}
