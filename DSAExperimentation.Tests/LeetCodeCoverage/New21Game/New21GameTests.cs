using DSAExperimentation.LeetCode.New21Game;

namespace DSAExperimentation.Tests.LeetCodeCoverage.New21Game;

// Harness only. Both strategies are New21GameSolution's - LeetCode's published
// examples (plus the k = 0 "Alice never draws" boundary the old test asserted, and
// two more boundaries) are stated once and replayed against each, so a failure names
// the strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm.
public sealed partial class New21GameTests
{
    private const int ProbabilityPrecision = 5;

    public static TheoryData<int, int, int, double> Examples =>
        new()
        {
            // LeetCode's first example: limit = 10, stopAt = 1, maxPts = 10 -> 1.0
            { 10, 1, 10, 1.0 },
            // LeetCode's second example: limit = 6, stopAt = 1, maxPts = 10 -> 0.6
            { 6, 1, 10, 0.6 },
            // LeetCode's third example: limit = 21, stopAt = 17, maxPts = 10 -> 0.73278
            { 21, 17, 10, 0.73278 },
            // stopAt = 0: Alice stops before drawing at all, at a total of 0 <= limit
            { 5, 0, 3, 1.0 },
            // A single winning draw out of maxPts equally likely ones
            { 1, 1, 10, 0.1 },
            // maxPts = 1: every draw adds exactly one, so the final total is stopAt
            { 21, 17, 1, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProbabilityByUnmemoizedRecursion_LeetCodeExamples_ReturnsExpectedProbability(
        int limit, int stopAt, int maxPts, double expected)
    {
        var actual = New21GameSolution.ProbabilityByUnmemoizedRecursion(limit, stopAt, maxPts);

        Assert.Equal(expected, actual, ProbabilityPrecision);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProbabilityByMemoizedRecursion_LeetCodeExamples_ReturnsExpectedProbability(
        int limit, int stopAt, int maxPts, double expected)
    {
        var actual = New21GameSolution.ProbabilityByMemoizedRecursion(limit, stopAt, maxPts);

        Assert.Equal(expected, actual, ProbabilityPrecision);
    }
}
