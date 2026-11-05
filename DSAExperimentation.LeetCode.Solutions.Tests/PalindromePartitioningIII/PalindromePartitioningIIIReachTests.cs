using DSAExperimentation.LeetCode.Solutions.PalindromePartitioningIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PalindromePartitioningIII;

public sealed partial class PalindromePartitioningIIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PalindromePartitioningIIIReach.IsWithin(new string('a', 27), 1));

    [Fact]
    public void IsWithin_TextLengthPastTheReach_IsFalse() => Assert.False(PalindromePartitioningIIIReach.IsWithin(new string('a', 28), 1));
}
