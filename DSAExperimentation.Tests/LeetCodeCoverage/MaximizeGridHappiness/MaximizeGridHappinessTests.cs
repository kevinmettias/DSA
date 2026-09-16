using DSAExperimentation.LeetCode.MaximizeGridHappiness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeGridHappiness;

// Harness only: both strategies are MaximizeGridHappinessSolution's - the unmemoized
// profile recursion and the same recursion routed through Memoizer - pinned here to
// LeetCode's three published examples plus the degenerate shapes the recursion has
// to get right at its edges: nobody to place, a single cell, a single row (the up
// neighbor never exists) and a full 2x2 of extroverts (every adjacency pays).
public sealed partial class MaximizeGridHappinessTests
{
    public static TheoryData<GridHappinessExample> Examples =>
        new()
        {
            { new GridHappinessExample(M: 2, N: 3, IntrovertsCount: 1, ExtrovertsCount: 2, Expected: 240) },
            { new GridHappinessExample(M: 3, N: 1, IntrovertsCount: 2, ExtrovertsCount: 1, Expected: 260) },
            { new GridHappinessExample(M: 2, N: 2, IntrovertsCount: 4, ExtrovertsCount: 0, Expected: 240) },

            // Both pools are empty, so the walk terminates before filling a cell.
            { new GridHappinessExample(M: 1, N: 1, IntrovertsCount: 0, ExtrovertsCount: 0, Expected: 0) },

            // One cell and more people than fit: the lone introvert wins it.
            { new GridHappinessExample(M: 1, N: 1, IntrovertsCount: 2, ExtrovertsCount: 1, Expected: 120) },

            // A single row, so only the left neighbor can ever exist: two adjacent
            // extroverts are worth more together (40 + 20 each) than apart.
            { new GridHappinessExample(M: 1, N: 2, IntrovertsCount: 0, ExtrovertsCount: 2, Expected: 120) },

            // Every cell filled with an extrovert, so all four adjacencies pay.
            { new GridHappinessExample(M: 2, N: 2, IntrovertsCount: 0, ExtrovertsCount: 4, Expected: 320) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxGridHappinessByBruteForceRecursion_LeetCodeExamples_ReturnsMaximumHappiness(
        GridHappinessExample example)
    {
        var actual = MaximizeGridHappinessSolution.GetMaxGridHappinessByBruteForceRecursion(
            example.M, example.N, example.IntrovertsCount, example.ExtrovertsCount);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxGridHappinessByMemoizedProfileDp_LeetCodeExamples_ReturnsMaximumHappiness(
        GridHappinessExample example)
    {
        var actual = MaximizeGridHappinessSolution.GetMaxGridHappinessByMemoizedProfileDp(
            example.M, example.N, example.IntrovertsCount, example.ExtrovertsCount);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct GridHappinessExample(
        int M, int N, int IntrovertsCount, int ExtrovertsCount, int Expected);
}
