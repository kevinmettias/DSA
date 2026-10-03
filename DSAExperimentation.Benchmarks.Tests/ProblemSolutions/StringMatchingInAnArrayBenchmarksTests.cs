using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StringMatchingInAnArrayBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same question - which of LC 1408's words are contained in another word - one by a raw
// char-index double loop, one by the prefix-function search, so a harness whose arms disagree
// is timing two different problems. Setup's words are built from their index alone, so the same
// word count must rebuild the same workload.
//
// Both arms return the contained words themselves. The derived expectation below pins them to
// the fixture's own structure, which is what keeps the check from being satisfied by two arms
// that are both wrong in the same way; LC 1408 accepts the words in any order, so they are
// compared as a set.
public sealed partial class StringMatchingInAnArrayBenchmarksTests
{
    private const int SmallestWordCount = 60;

    // Restated from the benchmark's generator: words of MinLength to MaxLength letters, in families
    // of FamilySize sharing a trailing letter from 'b' on.
    private const int MinLength = 6;
    private const int MaxLength = 30;
    private const int FamilySize = MaxLength - MinLength + 1;
    private const char FirstTrailingLetter = 'b';

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().NaiveNestedLoop()),
            AnswerGraphText.Of(BuildHarness().NaiveNestedLoop()));

    [Fact]
    public void NaiveNestedLoop_AdversarialWords_FindsEveryWordButEachFamilysLongest() =>
        AssertEveryWordButEachFamilysLongest(BuildHarness().NaiveNestedLoop());

    [Fact]
    public void KmpSubstringSearch_AdversarialWords_FindsEveryWordButEachFamilysLongest() =>
        AssertEveryWordButEachFamilysLongest(BuildHarness().KmpSubstringSearch());

    // Within a family a word is a run of 'a's and the family's letter, so it is a substring of every
    // longer word in its family and of no word in another: every word is contained except the last,
    // longest one of each family.
    private static void AssertEveryWordButEachFamilysLongest(List<string> contained)
    {
        var expected = Enumerable.Range(0, SmallestWordCount)
            .Where(index => !IsLongestOfItsFamily(index))
            .Select(Word)
            .ToList();

        Assert.Equal(expected.Count, contained.Count);
        Assert.Equal(expected.Order(StringComparer.Ordinal), contained.Order(StringComparer.Ordinal));
    }

    private static bool IsLongestOfItsFamily(int index) =>
        index % FamilySize == FamilySize - 1 || index == SmallestWordCount - 1;

    private static string Word(int index) =>
        new string('a', MinLength + (index % FamilySize) - 1) + (char)(FirstTrailingLetter + (index / FamilySize));

    private static StringMatchingInAnArrayBenchmarks BuildHarness()
    {
        var harness = new StringMatchingInAnArrayBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
