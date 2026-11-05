using DSAExperimentation.LeetCode.Solutions.FindTheNumberOfSubsequencesWithEqualGcd;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheNumberOfSubsequencesWithEqualGcd;

public sealed partial class FindTheNumberOfSubsequencesWithEqualGcdReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheNumberOfSubsequencesWithEqualGcdReach.IsWithin(new int[13]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(FindTheNumberOfSubsequencesWithEqualGcdReach.IsWithin(new int[14]));
}
