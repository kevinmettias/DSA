using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StepsToMakeArrayNonDecreasingBenchmarks (ARCHITECTURE 17.9): both arms
// answer the same question - how many removal rounds LC 2289 needs - one by simulating the
// rounds, one by a single monotonic-stack sweep, so a harness whose arms disagree is timing
// two different problems. Setup's value sequence is seeded, so the same length must rebuild
// the same workload.
public sealed partial class StepsToMakeArrayNonDecreasingBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SimulateRounds()),
            AnswerText.Of(BuildHarness().SimulateRounds()));

    [Fact]
    public void SimulateRounds_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SimulateRounds(), harness.MonotonicStackSweep());
    }

    [Fact]
    public void MonotonicStackSweep_AgreesWithSimulateRounds()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackSweep(), harness.SimulateRounds());
    }

    private static StepsToMakeArrayNonDecreasingBenchmarks BuildHarness()
    {
        var harness = new StepsToMakeArrayNonDecreasingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
