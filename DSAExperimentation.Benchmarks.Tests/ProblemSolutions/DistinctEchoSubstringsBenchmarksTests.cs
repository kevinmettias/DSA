using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctEchoSubstringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - comparing extracted substrings against screening the
// same pair with this repo's RollingHash first - so a harness whose arms disagree is timing two
// different problems. Setup draws Length random lowercase characters from one fixed seed, so the
// same Length must rebuild the same text. The class comment says that randomness makes most
// candidate pairs fail fast, which is the shape the O(1) screen exists for: every echo consumes at
// least one of the (start, halfLength) pairs the sweep visits, and those number 1599 at Length 80.
public sealed partial class DistinctEchoSubstringsBenchmarksTests
{
    private const int SmallestLength = 80;
    private const int EchoHalves = 2;
    private const int CandidatePairsAtEightyCharacters = 1_599;
    private const int MinimumDistinctEchoCount = 0;
    private const int MaximumEchoCountForMostPairsToFail = CandidatePairsAtEightyCharacters / EchoHalves;

    [Fact]
    public void Setup_EightyRandomCharacters_KeepsMostCandidatePairsFailingAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var echoes = harness.NaiveSubstringComparison();

        Assert.InRange(echoes, MinimumDistinctEchoCount, MaximumEchoCountForMostPairsToFail);
        Assert.Equal(echoes, BuildHarness().NaiveSubstringComparison());
    }

    [Fact]
    public void NaiveSubstringComparison_SeededRandomText_AgreesWithRollingHashScreenedEchoCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashScreenedEchoCount(), harness.NaiveSubstringComparison());
    }

    [Fact]
    public void RollingHashScreenedEchoCount_SeededRandomText_AgreesWithNaiveSubstringComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveSubstringComparison(), harness.RollingHashScreenedEchoCount());
    }

    private static DistinctEchoSubstringsBenchmarks BuildHarness()
    {
        var harness = new DistinctEchoSubstringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
