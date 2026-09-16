using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaxMinPartitionWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3578's
// array being the only input either strategy needs, with values spread across the problem's own range
// so windows have a real min/max spread to partition on.
public sealed partial class MaxMinPartitionWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 3578; // LC problem number
    private const int MinValue = 1;
    private const int ValueBound = 1000; // exclusive
    private const int DistinctValueFloor = 2;

    [Fact]
    public void BuildNums_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, MaxMinPartitionWorkloads.BuildNums(Length, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysWithinTheClosedRange() =>
        Assert.All(
            MaxMinPartitionWorkloads.BuildNums(Length, Seed),
            value => Assert.InRange(value, MinValue, ValueBound - 1));

    // An all-equal array would make every window's min and max coincide, so the partition the
    // strategies are measured on would have no spread to cut against.
    [Fact]
    public void BuildNums_Values_AreNotAllTheSame()
    {
        var nums = MaxMinPartitionWorkloads.BuildNums(Length, Seed);

        Assert.InRange(nums.Distinct().Count(), DistinctValueFloor, Length);
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            MaxMinPartitionWorkloads.BuildNums(Length, Seed),
            MaxMinPartitionWorkloads.BuildNums(Length, Seed));
}
