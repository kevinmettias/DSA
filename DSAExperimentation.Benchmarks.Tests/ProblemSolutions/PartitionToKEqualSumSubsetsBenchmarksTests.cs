using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionToKEqualSumSubsetsBenchmarks (ARCHITECTURE 17.9): its two arms are
// PartitionToKEqualSumSubsetsSolution's, competing searches for the same yes/no question - hand
// -rolled per-bucket backtracking against the generic bucket-filling backtrack - so a harness
// whose arms disagree is timing two different problems. Setup interleaves BucketCount copies of
// 1..NumbersPerSubset and shuffles them, so a perfect split always exists (each bucket can
// re-assemble one copy) whatever the seed; the tests below assert that existence as well as the
// agreement, which is what keeps a shared "false" from passing as agreement.
public sealed partial class PartitionToKEqualSumSubsetsBenchmarksTests
{
    private const int SmallestBucketCount = 3;

    [Fact]
    public void Setup_SameBucketCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanPartitionKSubsetsByNaiveBacktracking(),
            BuildHarness().CanPartitionKSubsetsByNaiveBacktracking());

    [Fact]
    public void CanPartitionKSubsetsByNaiveBacktracking_ShuffledEqualSubsets_AgreesWithCanPartitionKSubsetsByGenericBacktrack()
    {
        var harness = BuildHarness();

        Assert.True(harness.CanPartitionKSubsetsByGenericBacktrack());
        Assert.Equal(
            harness.CanPartitionKSubsetsByGenericBacktrack(),
            harness.CanPartitionKSubsetsByNaiveBacktracking());
    }

    [Fact]
    public void CanPartitionKSubsetsByGenericBacktrack_ShuffledEqualSubsets_AgreesWithCanPartitionKSubsetsByNaiveBacktracking()
    {
        var harness = BuildHarness();

        Assert.True(harness.CanPartitionKSubsetsByNaiveBacktracking());
        Assert.Equal(
            harness.CanPartitionKSubsetsByNaiveBacktracking(),
            harness.CanPartitionKSubsetsByGenericBacktrack());
    }

    private static PartitionToKEqualSumSubsetsBenchmarks BuildHarness()
    {
        var harness = new PartitionToKEqualSumSubsetsBenchmarks { BucketCount = SmallestBucketCount };
        harness.Setup();

        return harness;
    }
}
