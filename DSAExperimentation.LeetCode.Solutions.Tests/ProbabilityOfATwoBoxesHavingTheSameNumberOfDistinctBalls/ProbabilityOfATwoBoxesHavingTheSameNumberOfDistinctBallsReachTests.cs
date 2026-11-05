using DSAExperimentation.LeetCode.Solutions.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

public sealed partial class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsReach.IsWithin(new int[7]));

    [Fact]
    public void IsWithin_BallsLengthPastTheReach_IsFalse() => Assert.False(ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsReach.IsWithin(new int[8]));
}
