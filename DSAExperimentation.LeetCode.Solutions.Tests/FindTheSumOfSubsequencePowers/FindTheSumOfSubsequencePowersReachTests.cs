using DSAExperimentation.LeetCode.Solutions.FindTheSumOfSubsequencePowers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheSumOfSubsequencePowers;

public sealed partial class FindTheSumOfSubsequencePowersReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheSumOfSubsequencePowersReach.IsWithin(new int[7], 1));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(FindTheSumOfSubsequencePowersReach.IsWithin(new int[8], 1));
}
