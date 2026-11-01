using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionToKEqualSumSubsetsBenchmarks (ARCHITECTURE 17.9): the baseline's
// verdict against the one the workload's shape fixes, which the generic check cannot supply - it holds
// the generic backtrack to the baseline, and a shared "false" would pass as agreement. Setup
// interleaves BucketCount copies of 1..NumbersPerSubset and shuffles them, so a perfect split always
// exists (each bucket can re-assemble one copy) whatever the seed.
public sealed partial class PartitionToKEqualSumSubsetsBenchmarksTests
{
    private const int SmallestBucketCount = 2;

    [Fact]
    public void CanPartitionKSubsetsByNaiveBacktracking_ShuffledEqualSubsets_FindsTheSplit() =>
        Assert.True(BuildHarness().CanPartitionKSubsetsByNaiveBacktracking());

    private static PartitionToKEqualSumSubsetsBenchmarks BuildHarness()
    {
        var harness = new PartitionToKEqualSumSubsetsBenchmarks { BucketCount = SmallestBucketCount };
        harness.Setup();

        return harness;
    }
}
