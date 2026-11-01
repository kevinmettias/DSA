using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountOfSmallerNumbersAfterSelfBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// the shape of the array Setup draws through CountOfSmallerNumbersAfterSelfWorkloads, which no arm's
// answer reveals.
public sealed partial class CountOfSmallerNumbersAfterSelfBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int RandomSeed = 315;

    // The fixture's own value bound: values are drawn from [-ValueBound, ValueBound).
    private const int ValueBound = 10_000;

    [Fact]
    public void Setup_SmallestLength_DrawsALengthLongArrayWithinTheValueBound()
    {
        var nums = CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(SmallestLength, RandomSeed);

        // The documented shape: exactly Length random values spanning both signs of the fixture's
        // bound, so the counts are a real permutation-of-ranks question rather than one value.
        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, -ValueBound, ValueBound - 1));
    }
}
