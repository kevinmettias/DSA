using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks (ARCHITECTURE
// 17.9): the generic check holds the arms to each other at the smallest word count, and agreement
// says only that the two walks stopped at the same word. This pins the baseline's answer to the
// problem's -1 the workload is built to force, and holds the largest sentence and the search word
// to LC 1455's length bounds, which no comparison between arms can establish.
public sealed partial class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarksTests
{
    private const int SmallestWordCount = 4;

    // The harness's largest sentence and the seed it is drawn from: sixteen words of three to seven
    // letters and their fifteen separators must stay inside LC 1455's 100-character sentence.
    private const int LargestWordCount = 16;
    private const int SentenceSeed = 1455;
    private const int MaxSentenceLength = 100;
    private const int MaxSearchWordLength = 10;

    // The problem's own "no word starts with the search word" answer, and the value Setup's
    // workload must produce: the scenario's search word is longer than any generated word and
    // outside the generated alphabet's runs, so no word in the built sentence can start with it,
    // which is precisely why the workload forces both strategies through the whole word list.
    private const int NoPrefixWordFound = -1;

    [Fact]
    public void StartsWithScan_UnmatchedSearchWord_ReportsNoPrefixWordFound() =>
        Assert.Equal(NoPrefixWordFound, BuildHarness().StartsWithScan());

    // BenchmarkArmsTests only builds the smallest size, so the largest one's length is pinned here.
    [Fact]
    public void Setup_LargestWordCount_BuildsASentenceInsideTheLengthBound() =>
        Assert.InRange(
            PrefixSentenceWorkloads.BuildSentence(LargestWordCount, SentenceSeed).Length,
            1,
            MaxSentenceLength);

    [Fact]
    public void UnmatchedSearchWord_Length_StaysInsideTheSearchWordBound() =>
        Assert.InRange(PrefixSentenceScenario.UnmatchedSearchWord.Length, 1, MaxSearchWordLength);

    private static CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks BuildHarness()
    {
        var harness = new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
