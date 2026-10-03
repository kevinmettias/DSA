using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StringMatchingInAnArrayBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same question - which of LC 1408's words are contained in another word - one by a raw
// char-index double loop, one by the prefix-function search, so a harness whose arms disagree
// is timing two different problems. Setup's words are strictly increasing in length, so the
// same word count must rebuild the same workload.
//
// Both arms return the contained words themselves. The derived expectation below pins them to
// the fixture's own structure, which is what keeps the check from being satisfied by two arms
// that are both wrong in the same way; LC 1408 accepts the words in any order, so they are
// compared as a set.
public sealed partial class StringMatchingInAnArrayBenchmarksTests
{
    private const int SmallestWordCount = 60;

    // Every word is 'a' * (MinLength + index - 1) + 'b', so word i is a substring of word j
    // exactly when i < j: all but the single longest word are contained in another one.
    private const int ExpectedContainedWordCount = SmallestWordCount - 1;

    // Restated from the benchmark's generator.
    private const int MinLength = 6;
    private const string TrailingLetter = "b";

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().NaiveNestedLoop()),
            AnswerGraphText.Of(BuildHarness().NaiveNestedLoop()));

    [Fact]
    public void NaiveNestedLoop_AdversarialWords_FindsEveryWordButTheLongest() =>
        AssertEveryWordButTheLongest(BuildHarness().NaiveNestedLoop());

    [Fact]
    public void KmpSubstringSearch_AdversarialWords_FindsEveryWordButTheLongest() =>
        AssertEveryWordButTheLongest(BuildHarness().KmpSubstringSearch());

    private static void AssertEveryWordButTheLongest(List<string> contained)
    {
        var expected = Enumerable.Range(0, ExpectedContainedWordCount)
            .Select(index => new string('a', MinLength + index - 1) + TrailingLetter);

        Assert.Equal(ExpectedContainedWordCount, contained.Count);
        Assert.Equal(expected.Order(StringComparer.Ordinal), contained.Order(StringComparer.Ordinal));
    }

    private static StringMatchingInAnArrayBenchmarks BuildHarness()
    {
        var harness = new StringMatchingInAnArrayBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
