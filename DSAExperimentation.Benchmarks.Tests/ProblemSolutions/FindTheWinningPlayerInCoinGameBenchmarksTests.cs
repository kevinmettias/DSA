using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheWinningPlayerInCoinGameBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The two coin counts are the entire input and both are [Params] properties, so
// there is no [GlobalSetup] to rebuild and the harness is the bare initializer plus the two counts.
//
// Both arms return a bare string, so the two calls are compared directly.
public sealed partial class FindTheWinningPlayerInCoinGameBenchmarksTests
{
    private const int SmallestSeventyFiveCoinCount = 1;
    private const int SmallestTenCoinCount = 4;

    [Fact]
    public void Simulation_AgreesWithTurnParity()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TurnParity(), harness.Simulation());
    }

    [Fact]
    public void TurnParity_AgreesWithSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Simulation(), harness.TurnParity());
    }

    private static FindTheWinningPlayerInCoinGameBenchmarks BuildHarness() =>
        new()
        {
            SeventyFiveCoinCount = SmallestSeventyFiveCoinCount,
            TenCoinCount = SmallestTenCoinCount,
        };
}
