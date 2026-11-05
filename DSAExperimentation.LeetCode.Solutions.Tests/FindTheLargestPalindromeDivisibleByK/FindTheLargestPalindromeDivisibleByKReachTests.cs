using DSAExperimentation.LeetCode.Solutions.FindTheLargestPalindromeDivisibleByK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheLargestPalindromeDivisibleByK;

public sealed partial class FindTheLargestPalindromeDivisibleByKReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheLargestPalindromeDivisibleByKReach.IsWithin(14, 1));

    [Fact]
    public void IsWithin_DigitCountPastTheReach_IsFalse() => Assert.False(FindTheLargestPalindromeDivisibleByKReach.IsWithin(15, 1));
}
