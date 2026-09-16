using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheShortestSuperstringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup builds the seeded word chain and its overlap matrix from WordCount alone,
// so the same WordCount must rebuild the same workload.
//
// The words are kept as a chain whose only maximum-overlap order is its generation order, so both
// arms have exactly one shortest superstring to return and the direct string comparison below is a
// comparison of two answers to the same question. On the random word list this benchmark used before,
// both [Params] word counts admitted several distinct shortest superstrings, on which the two arms
// legitimately disagreed.
public sealed partial class FindTheShortestSuperstringBenchmarksTests
{
    private const int SmallestWordCount = 6;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForcePermutations(),
            BuildHarness().BruteForcePermutations());

    [Fact]
    public void BruteForcePermutations_AgreesWithMemoizedBitmaskDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedBitmaskDp(), harness.BruteForcePermutations());
    }

    [Fact]
    public void MemoizedBitmaskDp_AgreesWithBruteForcePermutations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePermutations(), harness.MemoizedBitmaskDp());
    }

    private static FindTheShortestSuperstringBenchmarks BuildHarness()
    {
        var harness = new FindTheShortestSuperstringBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
