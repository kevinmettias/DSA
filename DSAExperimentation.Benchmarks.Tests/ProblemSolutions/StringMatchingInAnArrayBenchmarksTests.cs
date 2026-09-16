using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StringMatchingInAnArrayBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same question - which of LC 1408's words are contained in another word - one by a raw
// char-index double loop, one by the prefix-function search, so a harness whose arms disagree
// is timing two different problems. Setup's words are strictly increasing in length, so the
// same word count must rebuild the same workload.
//
// Both arms return the contained-word count rather than the list, so agreement witnesses that
// both found the same NUMBER of contained words, not that they agree word for word. The
// derived expectation below pins that count to the fixture's own structure, which is what keeps
// the check from being satisfied by two arms that are both wrong in the same way.
public sealed partial class StringMatchingInAnArrayBenchmarksTests
{
    private const int SmallestWordCount = 60;

    // Every word is 'a' * (MinLength + index - 1) + 'b', so word i is a substring of word j
    // exactly when i < j: all but the single longest word are contained in another one.
    private const int ExpectedContainedWordCount = SmallestWordCount - 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaiveNestedLoop()),
            AnswerText.Of(BuildHarness().NaiveNestedLoop()));

    [Fact]
    public void NaiveNestedLoop_AgreesWithKmpSubstringSearch()
    {
        var harness = BuildHarness();
        var naive = harness.NaiveNestedLoop();

        Assert.Equal(naive, harness.KmpSubstringSearch());
        Assert.Equal(ExpectedContainedWordCount, naive);
    }

    [Fact]
    public void KmpSubstringSearch_AgreesWithNaiveNestedLoop()
    {
        var harness = BuildHarness();
        var prefixFunction = harness.KmpSubstringSearch();

        Assert.Equal(prefixFunction, harness.NaiveNestedLoop());
        Assert.Equal(ExpectedContainedWordCount, prefixFunction);
    }

    private static StringMatchingInAnArrayBenchmarks BuildHarness()
    {
        var harness = new StringMatchingInAnArrayBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
