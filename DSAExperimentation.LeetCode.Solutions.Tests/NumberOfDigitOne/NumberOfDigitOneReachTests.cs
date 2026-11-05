using DSAExperimentation.LeetCode.Solutions.NumberOfDigitOne;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfDigitOne;

public sealed partial class NumberOfDigitOneReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfDigitOneReach.IsWithin(10_000_000));

    [Fact]
    public void IsWithin_UpperBoundPastTheReach_IsFalse() => Assert.False(NumberOfDigitOneReach.IsWithin(10_000_001));
}
