using DSAExperimentation.LeetCode.Solutions.LongestBinarySubsequenceLessThanOrEqualToK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestBinarySubsequenceLessThanOrEqualToK;

public sealed partial class LongestBinarySubsequenceLessThanOrEqualToKReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(LongestBinarySubsequenceLessThanOrEqualToKReach.IsWithin(new string('a', 20), 1));

    [Fact]
    public void IsWithin_BitsLengthPastTheReach_IsFalse() => Assert.False(LongestBinarySubsequenceLessThanOrEqualToKReach.IsWithin(new string('a', 21), 1));
}
