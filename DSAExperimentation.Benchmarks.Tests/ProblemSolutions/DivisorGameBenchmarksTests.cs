using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivisorGameBenchmarks (ARCHITECTURE 17.9): an expected verdict known without
// either arm. Alice wins exactly when the starting position is even, and the benchmark's smallest
// parameter (100) is even, so the answer is true.
public sealed partial class DivisorGameBenchmarksTests
{
    private const int SmallestStartingPosition = 100;
    private const bool ExpectedWinForAnEvenStartingPosition = true;

    [Fact]
    public void CanAliceWinByParityFormula_EvenStartingPosition_Wins() =>
        Assert.Equal(ExpectedWinForAnEvenStartingPosition, BuildHarness().CanAliceWinByParityFormula());

    private static DivisorGameBenchmarks BuildHarness() => new() { StartingPosition = SmallestStartingPosition };
}
