using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumCircularSubarrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) brute force over every circular subarray
// against the two-Kadane complement trick - so a harness whose arms disagree is timing two different
// problems. Setup draws the values from one fixed seed, so the same Length must rebuild the same
// workload; otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumSumCircularSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().TwoPassKadane(), BuildHarness().TwoPassKadane());

    [Fact]
    public void BruteForceAllCircularSubarrays_SignedSeededRun_AgreesWithTwoPassKadane()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoPassKadane(), harness.BruteForceAllCircularSubarrays());
    }

    [Fact]
    public void TwoPassKadane_SignedSeededRun_AgreesWithBruteForceAllCircularSubarrays()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllCircularSubarrays(), harness.TwoPassKadane());
    }

    private static MaximumSumCircularSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumSumCircularSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
