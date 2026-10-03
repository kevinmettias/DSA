using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationsIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the k-th parity-alternating permutation of 1..PermutationLength -
// so a harness whose arms disagree is timing two different problems. Both measured lengths are
// asserted: at the larger one the rank is inside the count of valid permutations and the arms must
// agree on the actual unranking walk, while at the smaller one the fixed rank exceeds that count,
// where the solution's own contract is the empty result and the arms agree on taking it.
public sealed partial class PermutationsIVBenchmarksTests
{
    private const int SmallestPermutationLength = 20;
    private const int LargestPermutationLength = 100;

    [Fact]
    public void BigIntegerRank_AgreesWithFenwickOrderStatistics()
    {
        Assert.Equal(
            AnswerGraphText.Of(Harness(SmallestPermutationLength).FenwickOrderStatistics()),
            AnswerGraphText.Of(Harness(SmallestPermutationLength).BigIntegerRank()));
        Assert.Equal(
            AnswerGraphText.Of(Harness(LargestPermutationLength).FenwickOrderStatistics()),
            AnswerGraphText.Of(Harness(LargestPermutationLength).BigIntegerRank()));
    }

    private static PermutationsIVBenchmarks Harness(int permutationLength) =>
        new() { PermutationLength = permutationLength };
}
