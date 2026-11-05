using DSAExperimentation.LeetCode.Solutions.NumberOfDistinctRollSequences;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfDistinctRollSequences;

public sealed partial class NumberOfDistinctRollSequencesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfDistinctRollSequencesReach.IsWithin(15));

    [Fact]
    public void IsWithin_SequenceLengthPastTheReach_IsFalse() => Assert.False(NumberOfDistinctRollSequencesReach.IsWithin(16));
}
