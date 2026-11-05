using DSAExperimentation.LeetCode.Solutions.New21Game;

namespace DSAExperimentation.LeetCode.Solutions.Tests.New21Game;

// Harness only. Both strategies are New21GameSolution's - the k = 0 "Alice never
// draws" boundary the old test asserted, and two more boundaries, are stated once and
// replayed against each, so a failure names the strategy that broke rather than
// reporting a disagreement between an anonymous test helper and an anonymous
// benchmark arm. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class New21GameSolutionTests
{
    private const int ProbabilityPrecision = 5;

    public static TheoryData<int, int, int, double> Cases =>
        new()
        {
            // stopAt = 0: Alice stops before drawing at all, at a total of 0 <= limit
            { 5, 0, 3, 1.0 },
            // A single winning draw out of maxPts equally likely ones
            { 1, 1, 10, 0.1 },
            // maxPts = 1: every draw adds exactly one, so the final total is stopAt
            { 21, 17, 1, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ProbabilityByUnmemoizedRecursion_Cases_ReturnsExpectedProbability(
        int limit, int stopAt, int maxPts, double expected)
    {
        var actual = New21GameSolution.ProbabilityByUnmemoizedRecursion(limit, stopAt, maxPts);

        Assert.Equal(expected, actual, ProbabilityPrecision);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ProbabilityByMemoizedRecursion_Cases_ReturnsExpectedProbability(
        int limit, int stopAt, int maxPts, double expected)
    {
        var actual = New21GameSolution.ProbabilityByMemoizedRecursion(limit, stopAt, maxPts);

        Assert.Equal(expected, actual, ProbabilityPrecision);
    }
}
