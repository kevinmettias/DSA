using DSAExperimentation.LeetCode.Solutions.FindTheMinimumCostArrayPermutation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheMinimumCostArrayPermutation;

public sealed partial class FindTheMinimumCostArrayPermutationReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheMinimumCostArrayPermutationReach.IsWithin(new int[10]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(FindTheMinimumCostArrayPermutationReach.IsWithin(new int[11]));
}
