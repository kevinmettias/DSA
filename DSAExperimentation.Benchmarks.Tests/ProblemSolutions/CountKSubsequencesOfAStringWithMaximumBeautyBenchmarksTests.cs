using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - enumerating every subset of the
// text's distinct characters against grouping them by frequency - so a harness whose arms disagree
// is timing two different problems. Setup seeds the text, so the same Length must rebuild it.
public sealed partial class CountKSubsequencesOfAStringWithMaximumBeautyBenchmarksTests
{
    private const int SmallestLength = 5_000;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: the text is drawn from a five-letter pool and Setup asks for a
        // subsequence of four distinct characters, so the pool is always large enough to supply
        // one and the count can never be the degenerate 0 an undersized alphabet would give.
        Assert.InRange(first.BruteForceCombinations(), 1L, long.MaxValue);
        Assert.Equal(first.BruteForceCombinations(), second.BruteForceCombinations());
    }

    [Fact]
    public void BruteForceCombinations_FiveLetterPool_AgreesWithGroupedFrequencyProduct()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GroupedFrequencyProduct(), harness.BruteForceCombinations());
    }

    [Fact]
    public void GroupedFrequencyProduct_FiveLetterPool_AgreesWithBruteForceCombinations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceCombinations(), harness.GroupedFrequencyProduct());
    }

    private static CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks BuildHarness()
    {
        var harness = new CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
