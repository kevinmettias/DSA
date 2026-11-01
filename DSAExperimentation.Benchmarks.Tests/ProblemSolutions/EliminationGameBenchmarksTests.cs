using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EliminationGameBenchmarks (ARCHITECTURE 17.9): a bound known without either
// arm. The class carries no [GlobalSetup]: the arms are handed NumberCount directly, and the survivor
// LC 390 reports is necessarily one of the numbers the board started with, so it always lies in
// [1, NumberCount].
public sealed partial class EliminationGameBenchmarksTests
{
    private const int SmallestNumberCount = 10_000;
    private const int FirstRemainingNumber = 1;

    [Fact]
    public void HeadStepArithmetic_TenThousandNumbers_SurvivesFromTheStartingBoard() =>
        Assert.InRange(BuildHarness().HeadStepArithmetic(), FirstRemainingNumber, SmallestNumberCount);

    private static EliminationGameBenchmarks BuildHarness() => new() { NumberCount = SmallestNumberCount };
}
