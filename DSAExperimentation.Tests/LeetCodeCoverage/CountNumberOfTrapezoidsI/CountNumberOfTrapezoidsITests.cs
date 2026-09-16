using DSAExperimentation.LeetCode.CountNumberOfTrapezoidsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfTrapezoidsI;

// Harness only. The horizontal-trapezoid counting itself is
// CountNumberOfTrapezoidsISolution's - this file just pins both strategies to
// LeetCode's published examples.
public sealed partial class CountNumberOfTrapezoidsITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 0], [2, 0], [3, 0], [2, 2], [3, 2]], 3 },
            { [[0, 0], [1, 0], [0, 1], [2, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTrapezoidsByBruteForce_LeetCodeExamples_ReturnsTrapezoidCountModuloLargePrime(
        int[][] points, int expected) =>
        Assert.Equal(expected, CountNumberOfTrapezoidsISolution.CountTrapezoidsByBruteForce(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTrapezoidsByHorizontalPairCounting_LeetCodeExamples_ReturnsTrapezoidCountModuloLargePrime(
        int[][] points, int expected) =>
        Assert.Equal(expected, CountNumberOfTrapezoidsISolution.CountTrapezoidsByHorizontalPairCounting(points));
}
