using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumTotalSubarrayValueIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the brute-force enumeration of candidate windows
// against the two-segment-tree heap search - so a harness whose arms disagree is timing two different
// problems. Setup builds the values and both segment trees from one fixed seed and a fixed K, so the
// same Size must rebuild the same workload; otherwise two published numbers were never comparable in
// the first place.
public sealed partial class MaximumTotalSubarrayValueIIBenchmarksTests
{
    private const int SmallestSize = 100;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededValueRun_AgreesWithSegmentTreeHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeHeap(), harness.BruteForce());
    }

    [Fact]
    public void SegmentTreeHeap_SeededValueRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeHeap());
    }

    private static MaximumTotalSubarrayValueIIBenchmarks BuildHarness()
    {
        var harness = new MaximumTotalSubarrayValueIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
