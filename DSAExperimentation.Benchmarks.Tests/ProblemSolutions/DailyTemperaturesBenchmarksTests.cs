using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DailyTemperaturesBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, so what stays here is the workload's documented shape. The reading's
// documented shape is one wait per day, each inside the day count, and the last day has nothing ahead
// of it at all.
public sealed partial class DailyTemperaturesBenchmarksTests
{
    private const int SmallestLength = 200;

    // The last day of the generated run has no warmer day after it.
    private const int ExpectedFinalWaitDays = 0;

    [Fact]
    public void BruteForceScan_TwoHundredSeededDays_WaitsWithinTheRunAndEndsAtZero()
    {
        var waitDays = BuildHarness().BruteForceScan();

        Assert.Equal(SmallestLength, waitDays.Length);
        Assert.All(waitDays, wait => Assert.InRange(wait, ExpectedFinalWaitDays, SmallestLength - 1));
        Assert.Equal(ExpectedFinalWaitDays, waitDays[^1]);
    }

    private static DailyTemperaturesBenchmarks BuildHarness()
    {
        var harness = new DailyTemperaturesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
