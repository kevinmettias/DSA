using DSAExperimentation.LeetCode.Solutions.KthSmallestInstructions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSmallestInstructions;

public sealed partial class KthSmallestInstructionsReachTests
{
    // C(15, 7) is 6,435 and C(16, 7) is 11,440.
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(KthSmallestInstructionsReach.IsWithin([7, 8], 1));

    [Fact]
    public void IsWithin_PathCountPastTheReach_IsFalse() => Assert.False(KthSmallestInstructionsReach.IsWithin([7, 9], 1));
}
