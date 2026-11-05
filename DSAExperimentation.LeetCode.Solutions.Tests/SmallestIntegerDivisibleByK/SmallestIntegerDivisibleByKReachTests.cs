using DSAExperimentation.LeetCode.Solutions.SmallestIntegerDivisibleByK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestIntegerDivisibleByK;

public sealed partial class SmallestIntegerDivisibleByKReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SmallestIntegerDivisibleByKReach.IsWithin(75_321));

    [Fact]
    public void IsWithin_DivisorPastTheReach_IsFalse() => Assert.False(SmallestIntegerDivisibleByKReach.IsWithin(75_322));
}
