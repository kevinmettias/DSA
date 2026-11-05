using DSAExperimentation.LeetCode.Solutions.TheNumberOfBeautifulSubsets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheNumberOfBeautifulSubsets;

public sealed partial class TheNumberOfBeautifulSubsetsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(TheNumberOfBeautifulSubsetsReach.IsWithin(new int[16], 1));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(TheNumberOfBeautifulSubsetsReach.IsWithin(new int[17], 1));
}
