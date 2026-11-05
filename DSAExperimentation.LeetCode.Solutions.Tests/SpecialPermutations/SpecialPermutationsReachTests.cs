using DSAExperimentation.LeetCode.Solutions.SpecialPermutations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpecialPermutations;

public sealed partial class SpecialPermutationsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SpecialPermutationsReach.IsWithin(new int[8]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(SpecialPermutationsReach.IsWithin(new int[9]));
}
