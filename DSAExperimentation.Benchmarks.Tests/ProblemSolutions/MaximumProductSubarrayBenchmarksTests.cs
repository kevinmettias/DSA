using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductSubarrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) all-subarrays brute force against the O(n)
// single pass tracking both a running min and a running max - so a harness whose arms disagree is
// timing two different problems. Setup draws the values from one fixed seed, so the same Length must
// rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumProductSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceAllSubarrays(), BuildHarness().BruteForceAllSubarrays());

    [Fact]
    public void BruteForceAllSubarrays_SignedSeededRun_AgreesWithMinMaxSinglePass()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MinMaxSinglePass(), harness.BruteForceAllSubarrays());
    }

    [Fact]
    public void MinMaxSinglePass_SignedSeededRun_AgreesWithBruteForceAllSubarrays()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllSubarrays(), harness.MinMaxSinglePass());
    }

    private static MaximumProductSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumProductSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
