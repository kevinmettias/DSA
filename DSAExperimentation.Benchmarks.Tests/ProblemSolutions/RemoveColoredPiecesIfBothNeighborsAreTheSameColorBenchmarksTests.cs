using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks (ARCHITECTURE
// 17.9). Arm agreement is BenchmarkArmsTests' job; this pins the verdict the workload is built to
// have. Setup splits Length into one Alice run and one Bob run of equal length, which fixes the
// verdict without either arm: each run's budget is max(L - 2, 0) moves, and equal budgets mean
// Alice, moving first, takes the last available piece and loses.
public sealed partial class RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void CanAliceWinByGameSimulation_TwoEqualColorRuns_ReportsAliceCannotWin() =>
        Assert.False(BuildHarness().CanAliceWinByGameSimulation());

    private static RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks BuildHarness()
    {
        var harness = new RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks
        {
            Length = SmallestLength,
        };
        harness.Setup();

        return harness;
    }
}
