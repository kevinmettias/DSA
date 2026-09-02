using DSAExperimentation.LeetCode.ManhattanDistancesOfAllArrangementsOfPieces;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ManhattanDistancesOfAllArrangementsOfPieces;

// Harness only. Both strategies are
// ManhattanDistancesOfAllArrangementsOfPiecesSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class ManhattanDistancesOfAllArrangementsOfPiecesTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            { 2, 2, 2, 8 },
            { 1, 4, 3, 20 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByBruteForceArrangements_LeetCodeExamples_ReturnsTotalManhattanDistance(
        int m, int n, int k, long expected) =>
        Assert.Equal(
            expected,
            ManhattanDistancesOfAllArrangementsOfPiecesSolution.SumByBruteForceArrangements(m, n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByPairwiseDistanceFormula_LeetCodeExamples_ReturnsTotalManhattanDistance(
        int m, int n, int k, long expected) =>
        Assert.Equal(
            expected,
            ManhattanDistancesOfAllArrangementsOfPiecesSolution.SumByPairwiseDistanceFormula(m, n, k));
}
