using DSAExperimentation.LeetCode.Solutions.FindTheCountOfGoodIntegers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheCountOfGoodIntegers;

public sealed partial class FindTheCountOfGoodIntegersReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheCountOfGoodIntegersReach.IsWithin(8, 1));

    [Fact]
    public void IsWithin_DigitCountPastTheReach_IsFalse() => Assert.False(FindTheCountOfGoodIntegersReach.IsWithin(9, 1));
}
