using DSAExperimentation.LeetCode.CountCollisionsOfMonkeysOnAPolygon;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountCollisionsOfMonkeysOnAPolygon;

// Harness only. Both strategies are CountCollisionsOfMonkeysOnAPolygonSolution's -
// the repeated-doubling arm that used to live untested as the benchmark baseline,
// and the exponentiation-by-squaring arm - pinned to LeetCode's published examples
// plus lengths large enough that 2^n has wrapped the modulus several times, which
// is where the two arms would drift if either handled the fold differently.
public sealed class CountCollisionsOfMonkeysOnAPolygonTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 3, 6 },
            { 4, 14 },
            { 5, 30 },
            { 10, 1_022 },
            { 1_000, 688_423_208 },
            { 100_000, 607_723_518 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByRepeatedMultiplication_LeetCodeExamples_ReturnsCollisionCount(
        int n, int expected) =>
        Assert.Equal(
            expected,
            CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByRepeatedMultiplication(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByExponentiationBySquaring_LeetCodeExamples_ReturnsCollisionCount(
        int n, int expected) =>
        Assert.Equal(
            expected,
            CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByExponentiationBySquaring(n));
}
