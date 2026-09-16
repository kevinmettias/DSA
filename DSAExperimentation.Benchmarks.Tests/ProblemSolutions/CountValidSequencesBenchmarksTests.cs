using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountValidSequencesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - recomputing each binomial from scratch against reading it out of
// a table built once - so a harness whose arms disagree is timing two different problems, not two
// ways of answering one.
//
// There is no seed here: the whole workload is the TargetSum parameter together with the two things
// Setup derives from it - length = TargetSum / 2 and a factorial table that reaches TargetSum. The
// only report either arm makes is the answer, so the documented shape is asserted through that
// answer's own documented value: LC 4002 counts sequences of length k = n / 2 of positive integers
// summing to n with an even product as C(n - 1, k - 1) - C((n + k) / 2 - 1, k - 1) mod 1e9+7, which
// for n = 1000 is C(999, 499) - C(749, 499). A length other than n / 2, or a table that stops short
// of n, changes that residue.
public sealed partial class CountValidSequencesBenchmarksTests
{
    private const int SmallestTargetSum = 1_000;

    private const int ExpectedEvenProductSequenceCount = 498_280_751;

    [Fact]
    public void Setup_SameTargetSum_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedEvenProductSequenceCount, BuildHarness().DirectBinomial());
        Assert.Equal(BuildHarness().DirectBinomial(), BuildHarness().DirectBinomial());
    }

    [Fact]
    public void DirectBinomial_HalfLengthSequences_AgreesWithPrecomputedFactorials()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedFactorials(), harness.DirectBinomial());
    }

    [Fact]
    public void PrecomputedFactorials_HalfLengthSequences_AgreesWithDirectBinomial()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DirectBinomial(), harness.PrecomputedFactorials());
    }

    private static CountValidSequencesBenchmarks BuildHarness()
    {
        var harness = new CountValidSequencesBenchmarks { TargetSum = SmallestTargetSum };
        harness.Setup();

        return harness;
    }
}
