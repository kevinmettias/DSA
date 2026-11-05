using DSAExperimentation.LeetCode.Solutions.NumberOfBeautifulIntegersInTheRange;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfBeautifulIntegersInTheRange;

public sealed partial class NumberOfBeautifulIntegersInTheRangeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfBeautifulIntegersInTheRangeReach.IsWithin(1, 9_876_543, 1));

    [Fact]
    public void IsWithin_HighPastTheReach_IsFalse() => Assert.False(NumberOfBeautifulIntegersInTheRangeReach.IsWithin(1, 9_876_544, 1));
}
