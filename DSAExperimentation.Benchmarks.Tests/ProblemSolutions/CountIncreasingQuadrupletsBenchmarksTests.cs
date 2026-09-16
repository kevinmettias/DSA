using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountIncreasingQuadrupletsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the four nested loops testing the inequality as the
// statement writes it against the Fenwick sweep over the inverted middle pair - so a harness whose
// arms disagree is timing two different problems. Both arms return a long, so they are compared
// directly. Setup builds a permutation of 1..Length from one fixed seed, so the same Length must
// rebuild the same permutation, and that shape bounds the answer: one quadruplet is one choice of
// four distinct indices, so no permutation can admit more than Length choose 4 of them.
public sealed partial class CountIncreasingQuadrupletsBenchmarksTests
{
    private const int SmallestLength = 8;

    // The divisor in Length choose 4: four indices, taken 4! ways.
    private const int QuadrupletIndexCount = 24;

    // A permutation need not admit a single increasing quadruplet.
    private const long FewestQuadruplets = 0L;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePermutationWithinTheIndexBound()
    {
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

        Assert.InRange(BuildHarness().FenwickTreeSweep(), FewestQuadruplets, FourIndexSelections(SmallestLength));
    }

    [Fact]
    public void BruteForce_EightElementPermutation_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.BruteForce());
    }

    [Fact]
    public void FenwickTreeSweep_EightElementPermutation_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.FenwickTreeSweep());
    }

    private static CountIncreasingQuadrupletsBenchmarks BuildHarness()
    {
        var harness = new CountIncreasingQuadrupletsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // Length choose 4: i < j < k < l is exactly a choice of four distinct indices.
    private static long FourIndexSelections(int length) =>
        (long)length * (length - 1) * (length - 2) * (length - 3) / QuadrupletIndexCount;
}
