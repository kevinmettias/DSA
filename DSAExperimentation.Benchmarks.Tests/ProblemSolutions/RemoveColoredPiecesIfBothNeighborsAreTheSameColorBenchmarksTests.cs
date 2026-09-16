using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks (ARCHITECTURE
// 17.9): both arms are that solution's, competing strategies for the same question - playing the game
// out move by move against counting each run's max(L - 2, 0) budget - so a harness whose arms
// disagree settles two different games. Setup splits Length into one Alice run and one Bob run of
// equal length, which fixes the verdict: equal budgets mean Alice, moving first, takes the last
// available piece and loses. That verdict is asserted beside the agreement, because two bools
// agreeing on `false` would otherwise witness only that both arms said something.
public sealed partial class RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanAliceWinByGameSimulation(),
            BuildHarness().CanAliceWinByGameSimulation());

    [Fact]
    public void CanAliceWinByGameSimulation_TwoEqualColorRuns_ReportsAliceCannotWin()
    {
        var harness = BuildHarness();
        var simulationVerdict = harness.CanAliceWinByGameSimulation();

        Assert.False(simulationVerdict);
        Assert.Equal(harness.CanAliceWinByRunLengthCounting(), simulationVerdict);
    }

    [Fact]
    public void CanAliceWinByRunLengthCounting_AgreesWithGameSimulation()
    {
        var harness = BuildHarness();
        var runLengthVerdict = harness.CanAliceWinByRunLengthCounting();

        Assert.False(runLengthVerdict);
        Assert.Equal(harness.CanAliceWinByGameSimulation(), runLengthVerdict);
    }

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
