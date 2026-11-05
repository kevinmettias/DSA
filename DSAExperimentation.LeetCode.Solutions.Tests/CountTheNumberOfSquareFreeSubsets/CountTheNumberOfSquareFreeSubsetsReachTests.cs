using DSAExperimentation.LeetCode.Solutions.CountTheNumberOfSquareFreeSubsets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountTheNumberOfSquareFreeSubsets;

public sealed partial class CountTheNumberOfSquareFreeSubsetsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountTheNumberOfSquareFreeSubsetsReach.IsWithin(new int[21]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(CountTheNumberOfSquareFreeSubsetsReach.IsWithin(new int[22]));
}
