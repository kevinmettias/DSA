using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumCostToDivideArrayIntoSubarraysWorkloads (ARCHITECTURE 17.7). The reading
// depends on LC 3500's two arrays being drawn independently from the problem's own value range, with
// enough variety that a subarray's cost is a genuine choice.
public sealed partial class MinimumCostToDivideArrayIntoSubarraysWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 3500; // LC problem number
    private const int MinValue = 1;
    private const int MaxValueInclusive = 1000;
    private const int DistinctValueFloor = 2;

    [Fact]
    public void Build_Length_ReturnsOneValueAndOneCostPerPosition()
    {
        var (nums, cost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);

        Assert.Equal(Length, nums.Length);
        Assert.Equal(Length, cost.Length);
    }

    [Fact]
    public void Build_EveryValueAndCost_StaysWithinTheClosedRange()
    {
        var (nums, cost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);

        Assert.All(nums, value => Assert.InRange(value, MinValue, MaxValueInclusive));
        Assert.All(cost, amount => Assert.InRange(amount, MinValue, MaxValueInclusive));
    }

    [Fact]
    public void Build_ValuesAndCosts_AreNotAllTheSame()
    {
        var (nums, cost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);

        Assert.InRange(nums.Distinct().Count(), DistinctValueFloor, Length);
        Assert.InRange(cost.Distinct().Count(), DistinctValueFloor, Length);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (nums, cost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);
        var (repeatNums, repeatCost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);

        Assert.Equal(nums, repeatNums);
        Assert.Equal(cost, repeatCost);
    }
}
