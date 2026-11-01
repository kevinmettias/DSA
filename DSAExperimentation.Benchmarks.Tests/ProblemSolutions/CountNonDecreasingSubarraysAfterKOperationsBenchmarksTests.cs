using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNonDecreasingSubarraysAfterKOperationsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// the shape of the array Setup draws through NonDecreasingSubarrayWorkloads, which no arm's answer
// reveals.
public sealed partial class CountNonDecreasingSubarraysAfterKOperationsBenchmarksTests
{
    private const int SmallestSize = 200;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int Seed = 3420;

    // The fixture's own value bound: values are drawn from [1, ValueUpperBound).
    private const int MaxValue = 999;

    [Fact]
    public void Setup_SmallestSize_DrawsASizeLongArrayOfBoundedValues()
    {
        var nums = NonDecreasingSubarrayWorkloads.BuildNums(SmallestSize, Seed);

        // The documented shape: a Size-long array of random values from a range tight enough that
        // subarrays land on both sides of the budget, instead of every one of them blowing it.
        Assert.Equal(SmallestSize, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, 1, MaxValue));
    }
}
