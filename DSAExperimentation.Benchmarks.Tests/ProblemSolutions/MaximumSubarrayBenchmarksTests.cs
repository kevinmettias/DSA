using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSubarrayBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) all-subarrays brute force against the O(n) Kadane
// scan - so a harness whose arms disagree is timing two different problems. Setup draws the values
// from one fixed seed, so the same Length must rebuild the same workload; otherwise two published
// numbers were never comparable in the first place.
public sealed partial class MaximumSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().KadaneSinglePass(), BuildHarness().KadaneSinglePass());

    [Fact]
    public void BruteForceAllSubarrays_SignedSeededRun_AgreesWithKadaneSinglePass()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KadaneSinglePass(), harness.BruteForceAllSubarrays());
    }

    [Fact]
    public void KadaneSinglePass_SignedSeededRun_AgreesWithBruteForceAllSubarrays()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllSubarrays(), harness.KadaneSinglePass());
    }

    private static MaximumSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
