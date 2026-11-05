using DSAExperimentation.LeetCode.Solutions.FrequenciesOfShortestSupersequences;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FrequenciesOfShortestSupersequences;

public sealed partial class FrequenciesOfShortestSupersequencesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FrequenciesOfShortestSupersequencesReach.IsWithin(new string[15]));

    [Fact]
    public void IsWithin_WordCountPastTheReach_IsFalse() => Assert.False(FrequenciesOfShortestSupersequencesReach.IsWithin(new string[16]));
}
