using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestMatchingSubstringBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same shortest-matching-substring question about the same text and pattern, so a harness whose arms
// disagree is timing two different texts. Setup builds the text from one fixed seed over the fixture's
// six-letter alphabet - small enough that every literal part of the pattern occurs - and parses the
// pattern and precomputes its occurrences from that same text, so the same TextLength must rebuild
// all three. Neither arm writes to any of them, so one harness instance is safe to call twice.
public sealed partial class ShortestMatchingSubstringBenchmarksTests
{
    private const int SmallestTextLength = 500;

    [Fact]
    public void Setup_SameTextLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceIndexOf(), BuildHarness().BruteForceIndexOf());

    [Fact]
    public void BruteForceIndexOf_SixLetterAlphabetText_AgreesWithKmpBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KmpBinarySearch(), harness.BruteForceIndexOf());
    }

    [Fact]
    public void KmpBinarySearch_SixLetterAlphabetText_AgreesWithBruteForceIndexOf()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceIndexOf(), harness.KmpBinarySearch());
    }

    private static ShortestMatchingSubstringBenchmarks BuildHarness()
    {
        var harness = new ShortestMatchingSubstringBenchmarks { TextLength = SmallestTextLength };
        harness.Setup();

        return harness;
    }
}
