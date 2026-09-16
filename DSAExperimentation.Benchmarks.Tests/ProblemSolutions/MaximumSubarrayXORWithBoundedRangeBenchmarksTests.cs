using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSubarrayXORWithBoundedRangeBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the O(n^2) scan over every valid run against the
// bit-trie segment sweep - so a harness whose arms disagree is timing two different problems. Setup
// draws the values from one fixed seed against a fixed [Low, High] band, so the same Length must
// rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumSubarrayXORWithBoundedRangeBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BitTrieSegments_SeededBoundedRangeRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BitTrieSegments());
    }

    [Fact]
    public void BruteForce_SeededBoundedRangeRun_AgreesWithBitTrieSegments()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitTrieSegments(), harness.BruteForce());
    }

    private static MaximumSubarrayXORWithBoundedRangeBenchmarks BuildHarness()
    {
        var harness = new MaximumSubarrayXORWithBoundedRangeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
