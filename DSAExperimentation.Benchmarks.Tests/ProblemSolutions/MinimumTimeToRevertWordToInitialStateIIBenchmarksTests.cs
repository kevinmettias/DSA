using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToRevertWordToInitialStateIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - testing every candidate shift character by
// character against the linear Z-function arm that exists to close that gap - so a harness whose arms
// disagree is timing two different problems. Both arms only read the word drawn in [GlobalSetup], so
// one harness instance is safe to call twice in either order. Setup builds that word from one fixed
// seed, so the same WordLength must rebuild the same word; otherwise two published numbers were never
// comparable in the first place.
public sealed partial class MinimumTimeToRevertWordToInitialStateIIBenchmarksTests
{
    private const int SmallestWordLength = 500;

    [Fact]
    public void Setup_SameWordLength_RebuildsTheSameWord() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoLetterAlphabetWord_AgreesWithZFunction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunction(), harness.BruteForce());
    }

    [Fact]
    public void ZFunction_TwoLetterAlphabetWord_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ZFunction());
    }

    private static MinimumTimeToRevertWordToInitialStateIIBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToRevertWordToInitialStateIIBenchmarks { WordLength = SmallestWordLength };
        harness.Setup();

        return harness;
    }
}
