using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrintWordsVerticallyBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the trimmed column rows of the same word list - so a harness whose
// arms disagree is timing two different problems. WordCount is the only [Params] axis and Setup
// derives the words from it, so the same WordCount must rebuild the same list.
//
// Both arms return only .Count of the built rows, which is the longest word's length whichever
// trimming strategy ran. That is a proxy: two arms that built different rows of the same length
// would still agree. It is what the class returns, so the agreement witnessed here is that both
// strategies produce the same number of rows, not that they produce the same rows.
public sealed partial class PrintWordsVerticallyBenchmarksTests
{
    private const int SmallestWordCount = 50;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWords() =>
        Assert.Equal(BuildHarness().ListCharTrimEnd(), BuildHarness().ListCharTrimEnd());

    [Fact]
    public void ListCharTrimEnd_AgreesWithDynamicArrayTrimTail()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicArrayTrimTail(), harness.ListCharTrimEnd());
    }

    [Fact]
    public void DynamicArrayTrimTail_AgreesWithListCharTrimEnd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListCharTrimEnd(), harness.DynamicArrayTrimTail());
    }

    private static PrintWordsVerticallyBenchmarks BuildHarness()
    {
        var harness = new PrintWordsVerticallyBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
