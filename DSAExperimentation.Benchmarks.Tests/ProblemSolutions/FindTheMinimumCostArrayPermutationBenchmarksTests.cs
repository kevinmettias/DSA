using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheMinimumCostArrayPermutationBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - trying every permutation against a bitmask memo
// over (visited set, last index) - so a harness whose arms disagree is timing two different
// problems. Both return the permutation itself, in the one order it is read in, so AnswerText.Of
// is the right rendering. Setup shuffles 0..PermutationSize-1 from one fixed seed, so the same
// PermutationSize must rebuild the same permutation.
public sealed partial class FindTheMinimumCostArrayPermutationBenchmarksTests
{
    private const int SmallestPermutationSize = 6;

    [Fact]
    public void Setup_SamePermutationSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceSearch()),
            AnswerText.Of(BuildHarness().BruteForceSearch()));

    [Fact]
    public void BruteForceSearch_SmallestPermutationSize_AgreesWithBitmaskMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BitmaskMemoization()),
            AnswerText.Of(harness.BruteForceSearch()));
    }

    [Fact]
    public void BitmaskMemoization_SmallestPermutationSize_AgreesWithBruteForceSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceSearch()),
            AnswerText.Of(harness.BitmaskMemoization()));
    }

    private static FindTheMinimumCostArrayPermutationBenchmarks BuildHarness()
    {
        var harness = new FindTheMinimumCostArrayPermutationBenchmarks { PermutationSize = SmallestPermutationSize };
        harness.Setup();

        return harness;
    }
}
