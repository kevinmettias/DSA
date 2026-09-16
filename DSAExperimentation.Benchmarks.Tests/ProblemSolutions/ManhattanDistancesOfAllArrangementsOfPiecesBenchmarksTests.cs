using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ManhattanDistancesOfAllArrangementsOfPiecesBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - enumerating every arrangement of
// the pieces outright against the closed-form sum over piece pairs - so a harness whose arms
// disagree is timing two different problems. Both arms return the summed pairwise Manhattan
// distance as a long, and the sum the problem asks for is unique. This class has no [Params] beyond
// PieceCount and no [GlobalSetup]: the grid is the two fixed constants and the workstation count is
// the parameter, so the harness is a bare initializer with nothing to seed.
public sealed partial class ManhattanDistancesOfAllArrangementsOfPiecesBenchmarksTests
{
    private const int SmallestPieceCount = 2;

    [Fact]
    public void BruteForceArrangements_TwoPiecesOnTheFourByFourGrid_AgreesWithPairwiseDistanceFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseDistanceFormula(), harness.BruteForceArrangements());
    }

    [Fact]
    public void PairwiseDistanceFormula_TwoPiecesOnTheFourByFourGrid_AgreesWithBruteForceArrangements()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceArrangements(), harness.PairwiseDistanceFormula());
    }

    private static ManhattanDistancesOfAllArrangementsOfPiecesBenchmarks BuildHarness() =>
        new() { PieceCount = SmallestPieceCount };
}
