using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumBalancedSubsequenceSumBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the largest balanced subsequence sum - so a harness whose
// arms disagree is timing two different problems. Setup draws the array from a fixed seed, so the
// same length must rebuild the same workload; neither arm mutates it.
public sealed partial class MaximumBalancedSubsequenceSumBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithSegmentTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeSweep());
    }

    [Fact]
    public void SegmentTreeSweep_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeSweep(), harness.BruteForce());
    }

    private static MaximumBalancedSubsequenceSumBenchmarks BuildHarness()
    {
        var harness = new MaximumBalancedSubsequenceSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
