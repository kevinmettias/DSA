using DSAExperimentation.LeetCode.Solutions.DigitOperationsToMakeTwoIntegersEqual;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DigitOperationsToMakeTwoIntegersEqual;

public sealed partial class DigitOperationsToMakeTwoIntegersEqualReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(DigitOperationsToMakeTwoIntegersEqualReach.IsWithin(1024, 1));

    [Fact]
    public void IsWithin_StartValuePastTheReach_IsFalse() => Assert.False(DigitOperationsToMakeTwoIntegersEqualReach.IsWithin(1025, 1));
}
