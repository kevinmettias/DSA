using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToRevertWordToInitialStateIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - testing every candidate shift character by
// character against answering the same question through the Z-function the O(n) arm builds - so a
// harness whose arms disagree is timing two different problems. Both arms only read the word drawn
// in [GlobalSetup], so one harness instance is safe to call twice in either order. Setup builds that
// word from one fixed seed, so the same WordLength must rebuild the same word; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class MinimumTimeToRevertWordToInitialStateIBenchmarksTests
{
    private const int SmallestWordLength = 10;

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

    private static MinimumTimeToRevertWordToInitialStateIBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToRevertWordToInitialStateIBenchmarks { WordLength = SmallestWordLength };
        harness.Setup();

        return harness;
    }
}
