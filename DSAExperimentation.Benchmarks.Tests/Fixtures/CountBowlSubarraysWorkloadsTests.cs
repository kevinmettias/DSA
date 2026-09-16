using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CountBowlSubarraysWorkloads (ARCHITECTURE 17.7). LC 3676 guarantees
// distinct elements, so the reading depends on the array really being a shuffle of a
// contiguous range rather than independent draws that could repeat a value neither strategy
// is specified for.
public sealed partial class CountBowlSubarraysWorkloadsTests
{
    private const int Size = 256;
    private const int Seed = 3676; // LC problem number
    private const int MinValue = 1;

    [Fact]
    public void BuildNums_Size_ReturnsOneValuePerPosition() =>
        Assert.Equal(Size, CountBowlSubarraysWorkloads.BuildNums(Size, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_IsDistinct()
    {
        var nums = CountBowlSubarraysWorkloads.BuildNums(Size, Seed);

        Assert.Equal(Size, nums.Distinct().Count());
    }

    [Fact]
    public void BuildNums_EveryValue_StaysWithinTheContiguousRange()
    {
        var nums = CountBowlSubarraysWorkloads.BuildNums(Size, Seed);

        Assert.All(nums, value => Assert.InRange(value, MinValue, Size));
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameShuffle() =>
        Assert.Equal(
            CountBowlSubarraysWorkloads.BuildNums(Size, Seed),
            CountBowlSubarraysWorkloads.BuildNums(Size, Seed));
}
