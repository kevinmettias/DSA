using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchSuggestionsSystemBenchmarks (ARCHITECTURE 17.9): both arms answer LC
// 1268's per-keystroke question about the same catalog and the same search word, so a harness whose
// arms disagree is timing two different problems. Setup generates the catalog and the word from one
// fixed seed, so the same WordLength must rebuild the same pair. The answer is a list with one entry
// per keystroke and the problem pins that outer order, which is what AnswerText.Of checks; the three
// suggestions inside each entry are themselves ordered lexicographically by the problem, and both
// arms build them that way, so neither rendering has to fall back to an unordered comparison.
public sealed partial class SearchSuggestionsSystemBenchmarksTests
{
    private const int SmallestWordLength = 50;

    [Fact]
    public void Setup_SameWordLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanPerKeystroke()),
            AnswerText.Of(BuildHarness().LinearScanPerKeystroke()));

    [Fact]
    public void LinearScanPerKeystroke_SharedPrefixCatalog_AgreesWithSortOnceThenBinarySearchPerKeystroke()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SortOnceThenBinarySearchPerKeystroke()),
            AnswerText.Of(harness.LinearScanPerKeystroke()));
    }

    [Fact]
    public void SortOnceThenBinarySearchPerKeystroke_SharedPrefixCatalog_AgreesWithLinearScanPerKeystroke()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanPerKeystroke()),
            AnswerText.Of(harness.SortOnceThenBinarySearchPerKeystroke()));
    }

    private static SearchSuggestionsSystemBenchmarks BuildHarness()
    {
        var harness = new SearchSuggestionsSystemBenchmarks { WordLength = SmallestWordLength };
        harness.Setup();

        return harness;
    }
}
