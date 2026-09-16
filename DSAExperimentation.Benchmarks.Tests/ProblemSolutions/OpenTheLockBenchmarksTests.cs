using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OpenTheLockBenchmarks (ARCHITECTURE 17.9): its two arms are competing searches
// for one minimum turn count - the mutation queue over a deadend set against the BFS over a built
// LockGraph - so a harness whose arms disagree is timing two different problems. Setup builds both
// prepared inputs from one seeded deadend list, so the same DeadendCount must rebuild the same
// set and graph; the smallest tuned count is used because the prepared inputs are rebuilt for every
// harness the tests compare.
public sealed partial class OpenTheLockBenchmarksTests
{
    private const int SmallestDeadendCount = 0;

    [Fact]
    public void Setup_SameDeadendCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_SmallestDeadendCount_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    [Fact]
    public void ReduceGraphBfs_SmallestDeadendCount_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    private static OpenTheLockBenchmarks BuildHarness()
    {
        var harness = new OpenTheLockBenchmarks { DeadendCount = SmallestDeadendCount };
        harness.Setup();

        return harness;
    }
}
