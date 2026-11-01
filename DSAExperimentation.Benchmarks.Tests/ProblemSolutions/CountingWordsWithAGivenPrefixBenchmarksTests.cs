using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountingWordsWithAGivenPrefixBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload at the smallest WordCount, so this asserts
// only the workload's documented shape, at the largest WordCount, which that check never builds.
//
// The word array is private and the match count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts words of a WordCount-long array, and
// with a single-letter prefix over a 26-letter alphabet the seeded run at the largest WordCount has a
// non-trivial, non-zero share of them. At the smallest, ten words, the share is expected to be zero
// or one, and this seed happens to give zero, so the non-zero check is made at the largest size.
public sealed partial class CountingWordsWithAGivenPrefixBenchmarksTests
{
    private const int LargestWordCount = 100;

    // A single-letter prefix over a 26-letter alphabet leaves the seeded run with a non-trivial
    // share of matches - the arm counts words, not flags, so a zero would mean the alphabet never
    // put the prefix at the front of any word.
    private const int FewestPrefixMatches = 1;

    [Fact]
    public void StartsWithScan_LargestWordCount_FindsSomeWordsWithThePrefix() =>
        Assert.InRange(BuildHarness(LargestWordCount).StartsWithScan(), FewestPrefixMatches, LargestWordCount);

    private static CountingWordsWithAGivenPrefixBenchmarks BuildHarness(int wordCount)
    {
        var harness = new CountingWordsWithAGivenPrefixBenchmarks { WordCount = wordCount };
        harness.Setup();

        return harness;
    }
}
