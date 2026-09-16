using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarraysDistinctElementSumOfSquaresIIBenchmarks (ARCHITECTURE 17.9):
// both arms answer the same question - LC 2916's sum of squared distinct counts over every
// subarray - one by the O(n^2) brute force, one by the range-Fenwick sweep, so a harness whose
// arms disagree is timing two different problems. Setup's values are seeded, so the same length
// must rebuild the same workload.
public sealed partial class SubarraysDistinctElementSumOfSquaresIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithRangeFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.RangeFenwickTreeSweep());
    }

    [Fact]
    public void RangeFenwickTreeSweep_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RangeFenwickTreeSweep(), harness.BruteForce());
    }

    private static SubarraysDistinctElementSumOfSquaresIIBenchmarks BuildHarness()
    {
        var harness = new SubarraysDistinctElementSumOfSquaresIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
