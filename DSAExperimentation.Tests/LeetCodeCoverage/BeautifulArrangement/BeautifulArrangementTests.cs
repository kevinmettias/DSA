using DSAExperimentation.LeetCode.BeautifulArrangement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulArrangement;

// Harness only: both strategies are BeautifulArrangementSolution's, the same
// generate-then-filter baseline and pruned Backtrack.Search composition
// BeautifulArrangementBenchmarks measures.
public sealed class BeautifulArrangementTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountArrangements_LeetCodeExamples_ReturnsKnownCountByGenerateThenFilter(int size, int expected)
        => Assert.Equal(expected, BeautifulArrangementSolution.CountByGenerateThenFilter(size));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountArrangements_LeetCodeExamples_ReturnsKnownCountByPrunedBacktracking(int size, int expected)
        => Assert.Equal(expected, BeautifulArrangementSolution.CountByPrunedBacktracking(size));
}
