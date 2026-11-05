using DSAExperimentation.LeetCode.Solutions.SplitArrayWithSameAverage;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SplitArrayWithSameAverage;

public sealed partial class SplitArrayWithSameAverageReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SplitArrayWithSameAverageReach.IsWithin(new int[19]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(SplitArrayWithSameAverageReach.IsWithin(new int[20]));
}
