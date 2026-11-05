using DSAExperimentation.LeetCode.Solutions.CountNumberOfBalancedPermutations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountNumberOfBalancedPermutations;

public sealed partial class CountNumberOfBalancedPermutationsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountNumberOfBalancedPermutationsReach.IsWithin(new string('a', 9)));

    [Fact]
    public void IsWithin_NumLengthPastTheReach_IsFalse() => Assert.False(CountNumberOfBalancedPermutationsReach.IsWithin(new string('a', 10)));
}
