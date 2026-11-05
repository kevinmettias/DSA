using DSAExperimentation.LeetCode.Solutions.PermutationSequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PermutationSequence;

public sealed partial class PermutationSequenceReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PermutationSequenceReach.IsWithin(9, 1));

    [Fact]
    public void IsWithin_DigitCountPastTheReach_IsFalse() => Assert.False(PermutationSequenceReach.IsWithin(10, 1));
}
