using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfSubarrayRangesBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfSubarrayRangesSolution's - the O(n^2) walk over every subarray against the contribution sweep
// over the same extremes - so a harness whose arms disagree is timing two different questions. Both
// answer with a bare long, and the values are a seeded draw over a signed range, so a rebuild at the
// same Length has to produce the same total.
public sealed partial class SumOfSubarrayRangesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededSignedValues_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_SeededSignedValues_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static SumOfSubarrayRangesBenchmarks BuildHarness()
    {
        var harness = new SumOfSubarrayRangesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
