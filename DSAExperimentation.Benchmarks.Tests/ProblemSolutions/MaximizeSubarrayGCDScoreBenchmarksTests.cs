using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeSubarrayGCDScoreBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the best subarray GCD score under the fixed doubling
// budget - so a harness whose arms disagree is timing two different problems. Setup draws the array
// from a fixed seed, so the same length must rebuild the same workload; neither arm mutates it.
public sealed partial class MaximizeSubarrayGCDScoreBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithBottleneckGcdScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BottleneckGcdScan());
    }

    [Fact]
    public void BottleneckGcdScan_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BottleneckGcdScan(), harness.BruteForce());
    }

    private static MaximizeSubarrayGCDScoreBenchmarks BuildHarness()
    {
        var harness = new MaximizeSubarrayGCDScoreBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
