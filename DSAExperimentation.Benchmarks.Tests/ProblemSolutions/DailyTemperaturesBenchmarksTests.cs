using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DailyTemperaturesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the per-day brute-force scan ahead against one monotonic-stack
// sweep - so a harness whose arms disagree is timing two different problems. Setup draws the days
// from one fixed seed inside LC 739's 30..100 range; the reading's documented shape is one wait per
// day, each inside the day count, and the last day has nothing ahead of it at all. The same Length
// must rebuild the same draws and with it the same waits.
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
            AnswerGraphText.Of(BuildHarness().BruteForceScan()),
            AnswerGraphText.Of(BuildHarness().BruteForceScan()));
    }

    [Fact]
    public void BruteForceScan_TwoHundredSeededDays_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.MonotonicStackSweep()),
            AnswerGraphText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void MonotonicStackSweep_TwoHundredSeededDays_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.BruteForceScan()),
            AnswerGraphText.Of(harness.MonotonicStackSweep()));
    }

    private static DailyTemperaturesBenchmarks BuildHarness()
    {
        var harness = new DailyTemperaturesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
