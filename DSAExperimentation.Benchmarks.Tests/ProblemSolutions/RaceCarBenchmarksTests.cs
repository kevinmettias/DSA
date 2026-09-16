using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RaceCarBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the fewest commands that drive the car to Target - so a harness whose
// arms disagree is timing two different problems. Both return the command count as a scalar, so
// the arms are compared directly. Setup pre-bounds the state space for one target and materializes
// the composed arm's graph from it; Target is the only seed either build takes, so the same Target
// must rebuild both.
public sealed partial class RaceCarBenchmarksTests
{
    private const int SmallestTarget = 6;

    [Fact]
    public void Setup_SameTarget_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_SmallestTarget_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    [Fact]
    public void ReduceGraphBfs_SmallestTarget_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    private static RaceCarBenchmarks BuildHarness()
    {
        var harness = new RaceCarBenchmarks { Target = SmallestTarget };
        harness.Setup();

        return harness;
    }
}
