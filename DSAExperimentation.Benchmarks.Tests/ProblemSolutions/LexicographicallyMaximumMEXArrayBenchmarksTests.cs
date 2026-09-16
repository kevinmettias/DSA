using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LexicographicallyMaximumMEXArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - rescanning each window's MEX from scratch against
// the frequency/pointer sweep that never rescans - so a harness whose arms disagree is timing two
// different problems. Setup draws the array from one fixed seed, so the same Length must rebuild the
// same numbers; otherwise two published numbers were never comparable in the first place.
public sealed partial class LexicographicallyMaximumMEXArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameNumbers() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_SeededNumbers_AgreesWithFrequencyPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FrequencyPointer()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void FrequencyPointer_SeededNumbers_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.FrequencyPointer()));
    }

    private static LexicographicallyMaximumMEXArrayBenchmarks BuildHarness()
    {
        var harness = new LexicographicallyMaximumMEXArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
