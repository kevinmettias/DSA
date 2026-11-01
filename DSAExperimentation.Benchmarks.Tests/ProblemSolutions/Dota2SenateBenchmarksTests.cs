using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for Dota2SenateBenchmarks (ARCHITECTURE 17.9): an expected winner the workload's
// shape decides without either arm. Setup seats every 'R' before every 'D': with each senator voting
// in index order and Radiant holding the whole front of the circle, the opening Radiant block bans
// every Dire senator before a single Dire senator acts, so Radiant wins.
public sealed partial class Dota2SenateBenchmarksTests
{
    private const int SmallestSenatorCount = 500;
    private const string ExpectedWinnerWhenEveryRadiantSeatPrecedesEveryDireSeat = "Radiant";

    [Fact]
    public void CircularRescanSimulation_HalfRadiantThenHalfDire_RadiantWins() =>
        Assert.Equal(
            ExpectedWinnerWhenEveryRadiantSeatPrecedesEveryDireSeat,
            BuildHarness().CircularRescanSimulation());

    private static Dota2SenateBenchmarks BuildHarness()
    {
        var harness = new Dota2SenateBenchmarks { SenatorCount = SmallestSenatorCount };
        harness.Setup();

        return harness;
    }
}
