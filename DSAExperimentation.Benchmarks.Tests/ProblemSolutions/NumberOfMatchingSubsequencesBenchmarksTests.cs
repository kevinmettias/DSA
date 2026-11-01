using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfMatchingSubsequencesBenchmarks (ARCHITECTURE 17.9): the baseline's count
// against the one the workload contract fixes, which the generic check cannot supply - it holds the
// waiting-bucket arm to the baseline, and arms that agree on a spurious match agree just as well.
// Every generated word ends in the excluded 26th letter while the text is drawn from the 25-letter
// alphabet that omits it, so no word is ever a subsequence and the count can only be zero.
public sealed partial class NumberOfMatchingSubsequencesBenchmarksTests
{
    private const int SmallestWordCount = 200;

    // MatchingSubsequenceWorkloads appends the one letter its text alphabet excludes to every word,
    // so a word can only match if an arm invents one.
    private const int ExpectedMatchCount = 0;

    [Fact]
    public void TwoPointerPerWord_UnmatchableWords_CountsNoMatch() =>
        Assert.Equal(ExpectedMatchCount, BuildHarness().TwoPointerPerWord());

    private static NumberOfMatchingSubsequencesBenchmarks BuildHarness()
    {
        var harness = new NumberOfMatchingSubsequencesBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
