using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GasStationBenchmarks (ARCHITECTURE 17.9): both arms are GasStationSolution's
// - the simulated lap from every candidate start against the greedy debt reset - so a harness whose
// arms disagree is timing two different questions. [GlobalSetup] both draws the amounts and then
// tops the last station's gas up to guarantee a solution exists, which is what lets the arms be
// compared at all: on an unsolvable circuit both must report the same sentinel, but here the
// workload is inside LeetCode 134's contract. Both arms answer with the start station, so agreement
// says the two strategies found the same one. The draw is a pure function of Length off one seed,
// so the same Length must rebuild the same gas and cost.
public sealed partial class GasStationBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForceSimulateEveryStart(),
            BuildHarness().BruteForceSimulateEveryStart());

    [Fact]
    public void BruteForceSimulateEveryStart_SolvableCircuit_AgreesWithGreedyDebtReset()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedyDebtReset(), harness.BruteForceSimulateEveryStart());
    }

    [Fact]
    public void GreedyDebtReset_SolvableCircuit_AgreesWithBruteForceSimulateEveryStart()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSimulateEveryStart(), harness.GreedyDebtReset());
    }

    private static GasStationBenchmarks BuildHarness()
    {
        var harness = new GasStationBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
