using DSAExperimentation.LeetCode.Solutions.ClosestSubsequenceSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ClosestSubsequenceSum;

public sealed partial class ClosestSubsequenceSumReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ClosestSubsequenceSumReach.IsWithin(new int[15], 1));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(ClosestSubsequenceSumReach.IsWithin(new int[16], 1));
}
