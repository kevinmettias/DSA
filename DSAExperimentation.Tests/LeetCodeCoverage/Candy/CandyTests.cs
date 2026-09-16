using DSAExperimentation.LeetCode.Candy;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Candy;

// Harness only: both strategies live in CandySolution and are asserted against the
// same examples, including flat and strictly monotonic ratings that the two-pass
// slope-constraint approach's forward/backward passes have to combine correctly.
public sealed partial class CandyTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { new[] { 1, 0, 2 }, 5 },
            { new[] { 1, 2, 2 }, 4 },
            { new[] { 1, 3, 4, 5, 2 }, 11 },
            { new[] { 1 }, 1 },
            { new[] { 1, 1, 1 }, 3 },
            { new[] { 3, 2, 1 }, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCandiesByRepeatedRelaxation_LeetCodeExamples_ReturnsMinimumCandies(int[] ratings, int expected) =>
        Assert.Equal(expected, CandySolution.MinCandiesByRepeatedRelaxation(ratings));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCandiesByTwoPassSlopeConstraints_LeetCodeExamples_ReturnsMinimumCandies(int[] ratings, int expected) =>
        Assert.Equal(expected, CandySolution.MinCandiesByTwoPassSlopeConstraints(ratings));
}
