using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfMatchingSubsequencesBenchmarks (ARCHITECTURE 17.9): both arms count how
// many of the generated words are subsequences of the generated text - the per-word two-pointer scan
// against the HashMap/Queue waiting-bucket pass - so a harness whose arms disagree is timing two
// different questions. Both take LeetCode's own (searchedText, words) shape, so Setup only has to
// generate the input, and the same WordCount must rebuild it.
//
// This workload's arm agreement is weak by construction and the oracle below is what carries the
// test: every generated word ends in the excluded 26th letter while the text is drawn from the
// 25-letter alphabet that omits it, so no word is ever a subsequence and both arms can only return
// zero. The zero is asserted from that workload contract rather than from either arm, so the test
// fails if an arm ever starts reporting a match - which is exactly what bare agreement could not
// distinguish.
public sealed partial class NumberOfMatchingSubsequencesBenchmarksTests
{
    private const int SmallestWordCount = 200;

    // MatchingSubsequenceWorkloads appends the one letter its text alphabet excludes to every word,
    // so a word can only match if an arm invents one.
    private const int ExpectedMatchCount = 0;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().TwoPointerPerWord(), BuildHarness().TwoPointerPerWord());

    [Fact]
    public void TwoPointerPerWord_UnmatchableWords_AgreesWithHashMapQueueBuckets()
    {
        var harness = BuildHarness();
        var perWord = harness.TwoPointerPerWord();

        Assert.Equal(ExpectedMatchCount, perWord);
        Assert.Equal(perWord, harness.HashMapQueueBuckets());
    }

    [Fact]
    public void HashMapQueueBuckets_UnmatchableWords_AgreesWithTwoPointerPerWord()
    {
        var harness = BuildHarness();
        var buckets = harness.HashMapQueueBuckets();

        Assert.Equal(ExpectedMatchCount, buckets);
        Assert.Equal(buckets, harness.TwoPointerPerWord());
    }

    private static NumberOfMatchingSubsequencesBenchmarks BuildHarness()
    {
        var harness = new NumberOfMatchingSubsequencesBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
