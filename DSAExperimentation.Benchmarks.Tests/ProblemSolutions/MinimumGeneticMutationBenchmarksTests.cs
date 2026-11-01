using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumGeneticMutationBenchmarks (ARCHITECTURE 17.9): a property of the
// workload's shape, which the generic check cannot see - arms that agree on the no-path sentinel agree
// just as well. HammingWorkloads builds a mutation chain whose last value is the target and whose
// values are exactly the bank, so the target is always reachable and neither arm may shortcut on the
// no-mutation-possible case.
public sealed partial class MinimumGeneticMutationBenchmarksTests
{
    private const int SmallestGeneCount = 3;

    // The sentinel both strategies report when the target gene is in no gene's reach; the chain keeps
    // every value in the bank, so this is only the value a broken workload would collapse onto.
    private const int NoMutationPathSentinel = -1;

    [Fact]
    public void MutationQueueBfs_SeededMutationChain_FindsAMutationPath() =>
        Assert.NotEqual(NoMutationPathSentinel, BuildHarness().MutationQueueBfs());

    private static MinimumGeneticMutationBenchmarks BuildHarness()
    {
        var harness = new MinimumGeneticMutationBenchmarks { GeneCount = SmallestGeneCount };
        harness.Setup();

        return harness;
    }
}
