using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DailyTemperaturesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the per-day brute-force scan ahead against one monotonic-stack
// sweep - so a harness whose arms disagree is timing two different problems. Setup derives the days
// from one fixed seed as a permutation of 1..Length, so no day's scan short-circuits early; the
// reading's documented shape is one wait per day, each inside the day count, and the last day has
// nothing ahead of it at all. The same Length must rebuild the same permutation and with it the same
// waits.
public sealed partial class DailyTemperaturesBenchmarksTests
{
    private const int SmallestLength = 200;

    // The last day of the generated run has no warmer day after it.
    private const int ExpectedFinalWaitDays = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        var waitDays = BuildHarness().BruteForceScan();

        Assert.Equal(SmallestLength, waitDays.Length);
        Assert.All(waitDays, wait => Assert.InRange(wait, ExpectedFinalWaitDays, SmallestLength - 1));
        Assert.Equal(ExpectedFinalWaitDays, waitDays[^1]);
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));
    }

    [Fact]
    public void BruteForceScan_TwoHundredSeededDays_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void MonotonicStackSweep_TwoHundredSeededDays_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    private static DailyTemperaturesBenchmarks BuildHarness()
    {
        var harness = new DailyTemperaturesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
