using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ArrayEqualToTargetWorkloads (ARCHITECTURE 17.7). A workload generator
// is measurement scaffolding, so what it owes a test is the shape the reading depends on: one
// target per value, every value inside the generated range, and a target kept inside the tuned
// delta that stops the brute-force arm walking millions of steps per element.
public sealed partial class ArrayEqualToTargetWorkloadsTests
{
    private const int Length = 64;
    private const int Seed = 3229; // LC problem number
    private const int MinValue = 1;
    private const int MaxValue = 200;
    private const int MaxDelta = 6;

    [Fact]
    public void BuildArrays_Length_ReturnsOneValueAndOneTargetPerIndex()
    {
        var (nums, target) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);

        Assert.Equal(Length, nums.Length);
        Assert.Equal(Length, target.Length);
    }

    [Fact]
    public void BuildArrays_EveryValue_StaysWithinTheGeneratedRange()
    {
        var (nums, target) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);

        Assert.All(nums, value => Assert.InRange(value, MinValue, MaxValue));
        Assert.All(target, value => Assert.True(value >= MinValue));
    }

    // The delta bound is what keeps the per-unit simulation arm finite, so it is the one
    // property of this workload the comparison itself rests on.
    [Fact]
    public void BuildArrays_EveryTarget_StaysWithinTheTunedDeltaOfItsValue()
    {
        var (nums, target) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);

        for (var i = 0; i < Length; i++)
        {
            Assert.InRange(Math.Abs(target[i] - nums[i]), 0, MaxDelta);
        }
    }

    [Fact]
    public void BuildArrays_SameSeed_ReturnsTheSameArrays()
    {
        var (nums, target) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);
        var (repeatNums, repeatTarget) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);

        Assert.Equal(nums, repeatNums);
        Assert.Equal(target, repeatTarget);
    }
}
