using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPalindromeBenchmarks (ARCHITECTURE 17.9): both arms build the shortest
// palindrome from the same value, so a harness whose arms disagree is timing two different values.
// Setup draws the letters from one fixed seed, so the same Length must rebuild the same value; neither
// arm mutates it, so one harness instance is safe to call twice in either order. The answer is a
// string, so AnswerText.Of renders it as one atomic value rather than as a character sequence.
public sealed partial class ShortestPalindromeBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaivePrefixScan()),
            AnswerText.Of(BuildHarness().NaivePrefixScan()));

    [Fact]
    public void NaivePrefixScan_RandomLowercaseValue_AgreesWithKmpFailureFunction()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.KmpFailureFunction()),
            AnswerText.Of(harness.NaivePrefixScan()));
    }

    [Fact]
    public void KmpFailureFunction_RandomLowercaseValue_AgreesWithNaivePrefixScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.NaivePrefixScan()),
            AnswerText.Of(harness.KmpFailureFunction()));
    }

    private static ShortestPalindromeBenchmarks BuildHarness()
    {
        var harness = new ShortestPalindromeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
