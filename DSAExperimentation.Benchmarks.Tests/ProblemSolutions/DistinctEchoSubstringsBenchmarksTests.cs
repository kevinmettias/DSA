using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctEchoSubstringsBenchmarks (ARCHITECTURE 17.9): a property of the
// workload's shape. Setup draws Length random lowercase characters from one fixed seed, and the
// class comment says that randomness makes most candidate pairs fail fast, which is the shape the
// O(1) screen exists for: every echo consumes at least one of the (start, halfLength) pairs the
// sweep visits, and those number 1599 at Length 80.
public sealed partial class DistinctEchoSubstringsBenchmarksTests
{
    private const int SmallestLength = 80;
    private const int EchoHalves = 2;
    private const int CandidatePairsAtEightyCharacters = 1_599;
    private const int MinimumDistinctEchoCount = 0;
    private const int MaximumEchoCountForMostPairsToFail = CandidatePairsAtEightyCharacters / EchoHalves;

    [Fact]
    public void NaiveSubstringComparison_EightyRandomCharacters_KeepsMostCandidatePairsFailing() =>
        Assert.InRange(
            BuildHarness().NaiveSubstringComparison(),
            MinimumDistinctEchoCount,
            MaximumEchoCountForMostPairsToFail);

    private static DistinctEchoSubstringsBenchmarks BuildHarness()
    {
        var harness = new DistinctEchoSubstringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
