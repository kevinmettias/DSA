using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheSumOfSubsequencePowersBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws from a fixed seed and both arms take the same fixed k, so the
// same Length must rebuild the same workload.
public sealed partial class FindTheSumOfSubsequencePowersBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithThresholdCounting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ThresholdCounting(), harness.BruteForce());
    }

    [Fact]
    public void ThresholdCounting_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ThresholdCounting());
    }

    private static FindTheSumOfSubsequencePowersBenchmarks BuildHarness()
    {
        var harness = new FindTheSumOfSubsequencePowersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
