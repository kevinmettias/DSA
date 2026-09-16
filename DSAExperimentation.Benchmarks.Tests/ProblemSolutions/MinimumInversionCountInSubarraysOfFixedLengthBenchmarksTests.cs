using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumInversionCountInSubarraysOfFixedLengthBenchmarks (ARCHITECTURE
// 17.9): both arms are MinimumInversionCountInSubarraysOfFixedLengthSolution's, the same
// methods MinimumInversionCountInSubarraysOfFixedLengthTests proves correct, and both return
// the smallest inversion count over every fixed-length window. Arms that disagree are timing
// two different problems.
//
// Each arm builds its own sliding state from the hoisted nums array, so one harness instance
// answers both arms and the comparison is a genuine same-input comparison.
public sealed partial class MinimumInversionCountInSubarraysOfFixedLengthBenchmarksTests
{
    // The smallest declared [Params] value: the brute force is quadratic in the window length,
    // so a shorter array is the cheaper way to reach the same comparison.
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BruteForce(),
            BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithSlidingWindowFenwick()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlidingWindowFenwick(), harness.BruteForce());
    }

    [Fact]
    public void SlidingWindowFenwick_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SlidingWindowFenwick());
    }

    private static MinimumInversionCountInSubarraysOfFixedLengthBenchmarks BuildHarness()
    {
        var harness = new MinimumInversionCountInSubarraysOfFixedLengthBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
