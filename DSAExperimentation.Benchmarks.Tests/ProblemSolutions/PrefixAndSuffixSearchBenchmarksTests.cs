using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrefixAndSuffixSearchBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the largest index of a word carrying both affixes - so a harness whose
// arms disagree is timing two different problems. WordCount is the only [Params] axis and Setup
// derives the words, the queries and the prepared index from it, so the same WordCount must rebuild
// the same workload.
//
// Each arm replays the whole query stream but returns only the answer to the last query, so the
// agreement witnessed here is narrow: it pins the two strategies against each other on one query, not
// on the stream. Every earlier query's answer is discarded before the benchmark returns.
public sealed partial class PrefixAndSuffixSearchBenchmarksTests
{
    private const int SmallestWordCount = 200;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameQueries() =>
        Assert.Equal(BuildHarness().LinearScanPerQuery(), BuildHarness().LinearScanPerQuery());

    [Fact]
    public void LinearScanPerQuery_AgreesWithPrecomputedHashMapLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedHashMapLookup(), harness.LinearScanPerQuery());
    }

    [Fact]
    public void PrecomputedHashMapLookup_AgreesWithLinearScanPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanPerQuery(), harness.PrecomputedHashMapLookup());
    }

    private static PrefixAndSuffixSearchBenchmarks BuildHarness()
    {
        var harness = new PrefixAndSuffixSearchBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
