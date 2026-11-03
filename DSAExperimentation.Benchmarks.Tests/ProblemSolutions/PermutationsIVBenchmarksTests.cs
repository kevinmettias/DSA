using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationsIVBenchmarks (ARCHITECTURE 17.9): arm agreement at the larger
// measured length, which the generic check - run at the smallest PermutationLength only - never
// reaches. At the smaller length the fixed rank exceeds the count of valid permutations, so the arms
// agree only on the empty result; at the larger one the rank is inside that count and the arms must
// agree on the actual unranking walk.
public sealed partial class PermutationsIVBenchmarksTests
{
    private const int LargestPermutationLength = 100;

    [Fact]
    public void BigIntegerRank_LargestPermutationLength_AgreesWithFenwickOrderStatistics() =>
        Assert.Equal(
            AnswerGraphText.Of(Harness(LargestPermutationLength).FenwickOrderStatistics()),
            AnswerGraphText.Of(Harness(LargestPermutationLength).BigIntegerRank()));

    private static PermutationsIVBenchmarks Harness(int permutationLength) =>
        new() { PermutationLength = permutationLength };
}
