using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPartitionsWithMaxMinDifferenceAtMostKBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// the shape of the array Setup draws through MaxMinPartitionWorkloads, which no arm's answer
// reveals.
public sealed partial class CountPartitionsWithMaxMinDifferenceAtMostKBenchmarksTests
{
    private const int SmallestLength = 500;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int NumsSeed = 3578;

    // The fixture's own value bound: values are drawn from [1, ValueBound).
    private const int MaxValue = 999;

    [Fact]
    public void Setup_SmallestLength_DrawsALengthLongArrayWithinTheValueBound()
    {
        var nums = MaxMinPartitionWorkloads.BuildNums(SmallestLength, NumsSeed);

        // The documented shape: the array's values stay well below the scenario's
        // MaxMinPartitionScenario.MaxMinDifference bound, so every window is valid and each
        // position's segment stretches back to the start of the array.
        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, 1, MaxValue));
    }
}
