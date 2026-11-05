using DSAExperimentation.LeetCode.Solutions.Candy;

namespace DSAExperimentation.LeetCode.Solutions.Tests.Candy;

// Harness only: both strategies live in CandySolution and are asserted against the
// same cases, flat and strictly decreasing ratings, which the two-pass
// slope-constraint approach's forward/backward passes have to combine correctly.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CandySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { new[] { 1, 1, 1 }, 3 },
            { new[] { 3, 2, 1 }, 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCandiesByRepeatedRelaxation_Cases_ReturnsMinimumCandies(int[] ratings, int expected) =>
        Assert.Equal(expected, CandySolution.MinCandiesByRepeatedRelaxation(ratings));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCandiesByTwoPassSlopeConstraints_Cases_ReturnsMinimumCandies(int[] ratings, int expected) =>
        Assert.Equal(expected, CandySolution.MinCandiesByTwoPassSlopeConstraints(ratings));
}
