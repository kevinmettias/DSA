using DSAExperimentation.LeetCode.Solutions.PrimePalindrome;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrimePalindrome;

public sealed partial class PrimePalindromeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PrimePalindromeReach.IsWithin(9_988_999));

    [Fact]
    public void IsWithin_LowerBoundPastTheReach_IsFalse() => Assert.False(PrimePalindromeReach.IsWithin(9_989_000));
}
