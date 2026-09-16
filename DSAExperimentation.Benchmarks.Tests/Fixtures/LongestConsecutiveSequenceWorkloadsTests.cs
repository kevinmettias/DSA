using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LongestConsecutiveSequenceWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 128's value range being half the array's length, so the values duplicate heavily and fragment
// into many runs - which is what exercises the "skip if the predecessor is present" check.
public sealed partial class LongestConsecutiveSequenceWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 128; // LC problem number
    private const int LowestValue = 0;
    private const int FewestDistinctValues = 1;
    private const int ValueRangeDivisor = 2; // the workload draws values from Length / 2, half the array

    [Fact]
    public void BuildArray_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, LongestConsecutiveSequenceWorkloads.BuildArray(Length, Seed).Length);

    [Fact]
    public void BuildArray_EveryValue_StaysInsideTheHalvedValueRange()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(Length, Seed);
        var spread = Length / ValueRangeDivisor;

        Assert.All(nums, value => Assert.InRange(value, LowestValue, spread - 1));
    }

    // The spread is half the length, so by pigeonhole at most half the positions can hold a value of
    // their own - the bounded shape the reading needs: many short runs rather than one long one.
    [Fact]
    public void BuildArray_DistinctValues_CannotExceedTheHalvedRange()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(Length, Seed);
        var spread = Length / ValueRangeDivisor;

        Assert.True(spread < Length);
        Assert.InRange(nums.Distinct().Count(), FewestDistinctValues, spread);
    }

    [Fact]
    public void BuildArray_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            LongestConsecutiveSequenceWorkloads.BuildArray(Length, Seed),
            LongestConsecutiveSequenceWorkloads.BuildArray(Length, Seed));
}
