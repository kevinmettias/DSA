using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TaskSchedulerIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the backward scan over every earlier task against the
// HashMap's one lookup and one write per task - so a harness whose arms disagree is timing two
// different problems. Both arms return the day count as a long, so they are compared directly.
// Setup builds the distinct-id array from Length alone, so the same Length must rebuild the same
// workload; every id is distinct, which is what forces the backward scan through its full walk.
public sealed partial class TaskSchedulerIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithHashMapOnePass()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapOnePass(), harness.BruteForce());
    }

    [Fact]
    public void HashMapOnePass_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.HashMapOnePass());
    }

    private static TaskSchedulerIIBenchmarks BuildHarness()
    {
        var harness = new TaskSchedulerIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
