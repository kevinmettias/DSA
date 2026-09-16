using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodTripletsInAnArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - counting each middle position's smaller-left and
// larger-right neighbours with direct inner loops against two FenwickTree sweeps - so a harness whose
// arms disagree is timing two different problems. Both arms return a long, so they are compared
// directly. Setup draws both permutations from one fixed seed, so the same Length must rebuild the
// same pair of arrays: both arms re-express nums1 in nums2's positions internally, so an arm handed
// a different workload would count a different permutation's triplets without either arm noticing.
public sealed partial class CountGoodTripletsInAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePermutationPair() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_TwoHundredElementPermutations_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.PairwiseScan());
    }

    [Fact]
    public void FenwickTreeSweep_TwoHundredElementPermutations_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.FenwickTreeSweep());
    }

    private static CountGoodTripletsInAnArrayBenchmarks BuildHarness()
    {
        var harness = new CountGoodTripletsInAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
