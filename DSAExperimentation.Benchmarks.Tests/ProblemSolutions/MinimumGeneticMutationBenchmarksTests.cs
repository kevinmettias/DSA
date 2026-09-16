using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumGeneticMutationBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a BCL Queue + HashSet over candidates generated on the fly against
// this repo's BFS over the materialized bank graph - so a harness whose arms disagree is timing two
// different banks. Both arms answer with an int mutation count, which they compare directly. HammingWorkloads
// builds a mutation chain whose last value is the target and whose values are exactly the bank, so the
// target is always reachable and neither arm may shortcut on the no-mutation-possible case. Setup draws
// the chain from one seeded stream, so the same GeneCount must rebuild the same bank.
public sealed partial class MinimumGeneticMutationBenchmarksTests
{
    private const int SmallestGeneCount = 200;

    // The sentinel both strategies report when the target gene is in no gene's reach; the chain keeps
    // every value in the bank, so this is only the value a broken workload would collapse onto.
    private const int NoMutationPathSentinel = -1;

    [Fact]
    public void Setup_SameGeneCount_RebuildsTheSameBank() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_SeededMutationChain_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.NotEqual(NoMutationPathSentinel, harness.MutationQueueBfs());
        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    [Fact]
    public void ReduceGraphBfs_SeededMutationChain_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    private static MinimumGeneticMutationBenchmarks BuildHarness()
    {
        var harness = new MinimumGeneticMutationBenchmarks { GeneCount = SmallestGeneCount };
        harness.Setup();

        return harness;
    }
}
