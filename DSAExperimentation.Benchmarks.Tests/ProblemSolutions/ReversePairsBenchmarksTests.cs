using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReversePairsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a pairwise count over the whole array against a coordinate
// compressed sweep over a Fenwick tree - so a harness whose arms disagree is timing two different
// problems. Setup draws the values from one fixed seed, so the same Length must rebuild the same
// array; otherwise two published numbers were never comparable in the first place.
//
// Both arms only read the hoisted array, so one harness is safe to call twice in either order and
// the single-harness rule holds.
public sealed partial class ReversePairsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_SeededValues_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.PairwiseScan());
    }

    [Fact]
    public void FenwickTreeSweep_SeededValues_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.FenwickTreeSweep());
    }

    private static ReversePairsBenchmarks BuildHarness()
    {
        var harness = new ReversePairsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
