using DSAExperimentation.LeetCode.MaximizeGridHappiness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeGridHappiness;

// Harness only: both strategies are MaximizeGridHappinessSolution's - the unmemoized
// profile recursion and the same recursion routed through Memoizer - pinned here to
// LeetCode's three published examples plus the degenerate shapes the recursion has
// to get right at its edges: nobody to place, a single cell, a single row (the up
// neighbor never exists) and a full 2x2 of extroverts (every adjacency pays).
public sealed class MaximizeGridHappinessTests
{
    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            { 2, 3, 1, 2, 240 },
            { 3, 1, 2, 1, 260 },
            { 2, 2, 4, 0, 240 },

            // Both pools are empty, so the walk terminates before filling a cell.
            { 1, 1, 0, 0, 0 },

            // One cell and more people than fit: the lone introvert wins it.
            { 1, 1, 2, 1, 120 },

            // A single row, so only the left neighbor can ever exist: two adjacent
            // extroverts are worth more together (40 + 20 each) than apart.
            { 1, 2, 0, 2, 120 },

            // Every cell filled with an extrovert, so all four adjacencies pay.
            { 2, 2, 0, 4, 320 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxGridHappinessByBruteForceRecursion_LeetCodeExamples_ReturnsMaximumHappiness(
        int m, int n, int introvertsCount, int extrovertsCount, int expected) =>
        Assert.Equal(
            expected,
            MaximizeGridHappinessSolution.GetMaxGridHappinessByBruteForceRecursion(
                m, n, introvertsCount, extrovertsCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxGridHappinessByMemoizedProfileDp_LeetCodeExamples_ReturnsMaximumHappiness(
        int m, int n, int introvertsCount, int extrovertsCount, int expected) =>
        Assert.Equal(
            expected,
            MaximizeGridHappinessSolution.GetMaxGridHappinessByMemoizedProfileDp(
                m, n, introvertsCount, extrovertsCount));
}
