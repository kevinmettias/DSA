using DSAExperimentation.LeetCode.Solutions.CountCollisionsOfMonkeysOnAPolygon;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountCollisionsOfMonkeysOnAPolygon;

// Harness only. Both strategies are CountCollisionsOfMonkeysOnAPolygonSolution's -
// the repeated-doubling arm that used to live untested as the benchmark baseline,
// and the exponentiation-by-squaring arm - pinned to two polygons small enough that
// 2^n stays below the modulus, plus lengths large enough that 2^n has wrapped the
// modulus several times, which is where the two arms would drift if either handled
// the fold differently. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountCollisionsOfMonkeysOnAPolygonSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            { 5, 30 },
            { 10, 1_022 },
            { 1_000, 688_423_208 },
            { 100_000, 607_723_518 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByRepeatedMultiplication_Cases_ReturnsCollisionCount(
        int monkeyCount, int expected) =>
        Assert.Equal(
            expected,
            CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByRepeatedMultiplication(monkeyCount));

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWaysByExponentiationBySquaring_Cases_ReturnsCollisionCount(
        int monkeyCount, int expected) =>
        Assert.Equal(
            expected,
            CountCollisionsOfMonkeysOnAPolygonSolution.NumberOfWaysByExponentiationBySquaring(monkeyCount));
}
