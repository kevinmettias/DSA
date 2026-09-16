using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for Dota2SenateBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the circle with a bool[] of banned flags against
// this repo's own Queue<int> per party - so a harness whose arms disagree is timing two different
// problems. Setup seats every 'R' before every 'D', and that shape decides the winner: with each
// senator voting in index order and Radiant holding the whole front of the circle, the opening
// Radiant block bans every Dire senator before a single Dire senator acts, so Radiant wins. Both
// arms read the one string Setup built, so the same SenatorCount must rebuild it.
public sealed partial class Dota2SenateBenchmarksTests
{
    private const int SmallestSenatorCount = 500;
    private const string ExpectedWinnerWhenEveryRadiantSeatPrecedesEveryDireSeat = "Radiant";

    [Fact]
    public void Setup_HalfRadiantThenHalfDire_SeatsEverySenatorAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var winner = harness.CircularRescanSimulation();

        Assert.Equal(ExpectedWinnerWhenEveryRadiantSeatPrecedesEveryDireSeat, winner);
        Assert.Equal(winner, BuildHarness().CircularRescanSimulation());
    }

    [Fact]
    public void CircularRescanSimulation_HalfRadiantThenHalfDire_AgreesWithTwoQueueSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoQueueSimulation(), harness.CircularRescanSimulation());
    }

    [Fact]
    public void TwoQueueSimulation_HalfRadiantThenHalfDire_AgreesWithCircularRescanSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CircularRescanSimulation(), harness.TwoQueueSimulation());
    }

    private static Dota2SenateBenchmarks BuildHarness()
    {
        var harness = new Dota2SenateBenchmarks { SenatorCount = SmallestSenatorCount };
        harness.Setup();

        return harness;
    }
}
