using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LastSubstringInLexicographicalOrderBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - pairwise suffix comparison against this
// repo's own SuffixArray - so a harness whose arms disagree is timing two different problems. Setup
// draws the text from one fixed seed over a small alphabet, so the same Length must rebuild the same
// text; otherwise two published numbers were never comparable in the first place.
public sealed partial class LastSubstringInLexicographicalOrderBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameText() =>
        Assert.Equal(BuildHarness().PairwiseComparison(), BuildHarness().PairwiseComparison());

    [Fact]
    public void PairwiseComparison_SeededSmallAlphabetText_AgreesWithSuffixArrayLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SuffixArrayLookup(), harness.PairwiseComparison());
    }

    [Fact]
    public void SuffixArrayLookup_SeededSmallAlphabetText_AgreesWithPairwiseComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseComparison(), harness.SuffixArrayLookup());
    }

    private static LastSubstringInLexicographicalOrderBenchmarks BuildHarness()
    {
        var harness = new LastSubstringInLexicographicalOrderBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
