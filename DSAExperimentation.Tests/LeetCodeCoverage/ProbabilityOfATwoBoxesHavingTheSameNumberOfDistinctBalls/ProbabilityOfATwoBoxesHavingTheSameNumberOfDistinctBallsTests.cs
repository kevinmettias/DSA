using DSAExperimentation.LeetCode.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

// Harness only. Both enumeration strategies are
// ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution's - this file
// just pins them to LeetCode's published examples plus two hand-checkable
// extremes the original test lacked: a deal that always matches ([2, 2], where
// every half/half split leaves one distinct colour on each side or two on both)
// and one that never can ([3, 1], where box 1 either holds a single colour or
// splits the majority colour and leaves box 2 with only it).
public sealed class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsTests
{
    // The probability is accumulated in double, so the published five-decimal
    // answers are compared well inside their own rounding.
    private const int Precision = 9;

    public static TheoryData<int[], double> Examples =>
        new()
        {
            { [1, 1], 1.0 },
            { [2, 1, 1], 2.0 / 3.0 },
            { [1, 2, 1, 2], 0.6 },
            { [3, 2, 1], 0.3 },
            { [2, 2], 1.0 },
            { [3, 1], 0.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetProbabilityByHandRolledRecursion_LeetCodeExamples_ReturnsMatchingDistinctCountProbability(
        int[] balls, double expected) =>
        Assert.Equal(
            expected,
            ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution
                .GetProbabilityByHandRolledRecursion(balls),
            precision: Precision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetProbabilityByBacktracking_LeetCodeExamples_ReturnsMatchingDistinctCountProbability(
        int[] balls, double expected) =>
        Assert.Equal(
            expected,
            ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution
                .GetProbabilityByBacktracking(balls),
            precision: Precision);
}
