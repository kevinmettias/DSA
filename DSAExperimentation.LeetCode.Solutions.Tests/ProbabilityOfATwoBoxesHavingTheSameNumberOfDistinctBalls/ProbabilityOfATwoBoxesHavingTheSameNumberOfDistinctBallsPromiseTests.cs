using DSAExperimentation.LeetCode.Solutions.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

public sealed partial class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 1], true },
            { [2, 1, 1], true },
            { [1, 2, 1, 2], true },
            { [1, 2, 2, 2, 2, 2, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheBallsSplitIntoTwoEqualBoxes(int[] balls, bool expected)
        => Assert.Equal(expected, ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsPromise.IsKeptBy(balls));
}
