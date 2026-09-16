using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSubarrayMinProductBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) brute force over every window's running
// minimum against the monotonic-stack sweep - so a harness whose arms disagree is timing two
// different problems. Setup permutes 1..Length from one fixed seed, so the same Length must rebuild
// the same workload; otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumSubarrayMinProductBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MonotonicStackSweep(), BuildHarness().MonotonicStackSweep());

    [Fact]
    public void BruteForce_PermutationOfOneToLength_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackSweep(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStackSweep_PermutationOfOneToLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStackSweep());
    }

    private static MaximumSubarrayMinProductBenchmarks BuildHarness()
    {
        var harness = new MaximumSubarrayMinProductBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
