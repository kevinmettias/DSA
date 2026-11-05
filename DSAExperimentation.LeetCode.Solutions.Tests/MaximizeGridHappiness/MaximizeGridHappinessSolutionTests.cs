using DSAExperimentation.LeetCode.Solutions.MaximizeGridHappiness;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximizeGridHappiness;

// Harness only: both strategies are MaximizeGridHappinessSolution's - the unmemoized
// profile recursion and the same recursion routed through Memoizer - pinned here to
// the degenerate shapes the recursion has to get right at its edges: a single cell, a
// single row (the up neighbor never exists) and a full 2x2 of extroverts (every
// adjacency pays). LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximizeGridHappinessSolutionTests
{
    public static TheoryData<GridHappinessCase> Cases =>
        new()
        {
            // One cell and more people than fit: the lone introvert wins it.
            { new GridHappinessCase(M: 1, N: 1, IntrovertsCount: 2, ExtrovertsCount: 1, Expected: 120) },

            // A single row, so only the left neighbor can ever exist: two adjacent
            // extroverts are worth more together (40 + 20 each) than apart.
            { new GridHappinessCase(M: 1, N: 2, IntrovertsCount: 0, ExtrovertsCount: 2, Expected: 120) },

            // Every cell filled with an extrovert, so all four adjacencies pay.
            { new GridHappinessCase(M: 2, N: 2, IntrovertsCount: 0, ExtrovertsCount: 4, Expected: 320) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetMaxGridHappinessByBruteForceRecursion_Cases_ReturnsMaximumHappiness(
        GridHappinessCase testCase)
    {
        var actual = MaximizeGridHappinessSolution.GetMaxGridHappinessByBruteForceRecursion(
            testCase.M, testCase.N, testCase.IntrovertsCount, testCase.ExtrovertsCount);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetMaxGridHappinessByMemoizedProfileDp_Cases_ReturnsMaximumHappiness(
        GridHappinessCase testCase)
    {
        var actual = MaximizeGridHappinessSolution.GetMaxGridHappinessByMemoizedProfileDp(
            testCase.M, testCase.N, testCase.IntrovertsCount, testCase.ExtrovertsCount);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case as one argument: the five values that describe it. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct GridHappinessCase(
        int M, int N, int IntrovertsCount, int ExtrovertsCount, int Expected);
}
