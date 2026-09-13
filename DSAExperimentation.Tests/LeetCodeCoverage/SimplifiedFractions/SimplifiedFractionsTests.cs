using DSAExperimentation.LeetCode.SimplifiedFractions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SimplifiedFractions;

// Harness only. Both strategies are SimplifiedFractionsSolution's - this file pins
// them to LeetCode's published examples, including n = 1 (no proper fraction
// exists at all) and n = 4, whose expected list omits the reducible 2/4.
public sealed class SimplifiedFractionsTests
{
    public static TheoryData<int, string[]> Examples =>
        new()
        {
            { 1, [] },
            { 2, ["1/2"] },
            { 3, ["1/2", "1/3", "2/3"] },
            { 4, ["1/2", "1/3", "2/3", "1/4", "3/4"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ListFractionsByTrialDivisionGcd_LeetCodeExamples_ReturnsEveryCoprimePairExactlyOnce(
        int n, string[] expected) =>
        Assert.Equal(expected, SimplifiedFractionsSolution.ListFractionsByTrialDivisionGcd(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ListFractionsByEuclideanGcd_LeetCodeExamples_ReturnsEveryCoprimePairExactlyOnce(
        int n, string[] expected) =>
        Assert.Equal(expected, SimplifiedFractionsSolution.ListFractionsByEuclideanGcd(n));
}
