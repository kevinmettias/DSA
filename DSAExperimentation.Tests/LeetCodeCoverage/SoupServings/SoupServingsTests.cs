using DSAExperimentation.LeetCode.SoupServings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SoupServings;

// Harness only. Both strategies are SoupServingsSolution's - LeetCode's published
// examples (plus the empty-pot and single-serving boundaries, and the large-amount
// case the old test asserted) are stated once and replayed against each, so a failure
// names the strategy that broke rather than reporting a disagreement between an
// anonymous test helper and an anonymous benchmark arm.
public sealed partial class SoupServingsTests
{
    private const int ProbabilityPrecision = 5;

    public static TheoryData<int, double> Examples =>
        new()
        {
            { 0, 0.5 },
            { 25, 0.625 },
            { 50, 0.625 },
            { 100, 0.71875 },
            { 10_000, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProbabilityByUnmemoizedRecursion_LeetCodeExamples_ReturnsExpectedProbability(
        int milliliters, double expected) =>
        Assert.Equal(
            expected,
            SoupServingsSolution.ProbabilityByUnmemoizedRecursion(milliliters),
            ProbabilityPrecision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProbabilityByMemoizedRecursion_LeetCodeExamples_ReturnsExpectedProbability(
        int milliliters, double expected) =>
        Assert.Equal(
            expected,
            SoupServingsSolution.ProbabilityByMemoizedRecursion(milliliters),
            ProbabilityPrecision);
}
