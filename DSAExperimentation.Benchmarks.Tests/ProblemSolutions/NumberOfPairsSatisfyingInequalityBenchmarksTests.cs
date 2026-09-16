using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfPairsSatisfyingInequalityBenchmarks (ARCHITECTURE 17.9): both arms count
// the same index pairs - the textbook O(n^2) pairwise scan against the FenwickTree sweep over
// coordinate-compressed differences - so a harness whose arms disagree is timing two different
// questions. The count is the problem's whole answer rather than a proxy. Setup draws nums1, nums2 and
// the diff from one seed, so the same Length must rebuild all three.
public sealed partial class NumberOfPairsSatisfyingInequalityBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.PairwiseScan());
    }

    [Fact]
    public void FenwickTreeSweep_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.FenwickTreeSweep());
    }

    private static NumberOfPairsSatisfyingInequalityBenchmarks BuildHarness()
    {
        var harness = new NumberOfPairsSatisfyingInequalityBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
