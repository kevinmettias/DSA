using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfValidWordsForEachPuzzleBenchmarks (ARCHITECTURE 17.9): both arms return
// one valid-word count per puzzle - the per-puzzle mask scan over every word against the HashMap arm
// that enumerates each puzzle's submasks once - so a harness whose arms disagree is timing two
// different questions. The returned list is the problem's whole answer rather than a proxy, and its
// outer order is the puzzle order LeetCode's own return value pins, so the default order-sensitive
// rendering is the right comparison. Setup generates words and puzzles from one seed, so the same
// WordCount must rebuild both.
public sealed partial class NumberOfValidWordsForEachPuzzleBenchmarksTests
{
    private const int SmallestWordCount = 200;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MaskComparison()),
            AnswerText.Of(BuildHarness().MaskComparison()));

    [Fact]
    public void MaskComparison_AgreesWithHashMapSubsetEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.HashMapSubsetEnumeration()),
            AnswerText.Of(harness.MaskComparison()));
    }

    [Fact]
    public void HashMapSubsetEnumeration_AgreesWithMaskComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MaskComparison()),
            AnswerText.Of(harness.HashMapSubsetEnumeration()));
    }

    private static NumberOfValidWordsForEachPuzzleBenchmarks BuildHarness()
    {
        var harness = new NumberOfValidWordsForEachPuzzleBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
