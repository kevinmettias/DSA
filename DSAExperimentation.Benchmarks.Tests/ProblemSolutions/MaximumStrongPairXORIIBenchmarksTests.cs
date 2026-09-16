using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumStrongPairXORIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) pairwise scan against the sorted-input bit
// trie buckets - so a harness whose arms disagree is timing two different problems. Setup draws the
// values from one fixed seed and sorts its own private clone for the trie arm, so the same Length
// must rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumStrongPairXORIIBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BitTrieBuckets_SortedCloneOfTheSameValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BitTrieBuckets());
    }

    [Fact]
    public void BruteForce_UnsortedOriginalValues_AgreesWithBitTrieBuckets()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitTrieBuckets(), harness.BruteForce());
    }

    private static MaximumStrongPairXORIIBenchmarks BuildHarness()
    {
        var harness = new MaximumStrongPairXORIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
