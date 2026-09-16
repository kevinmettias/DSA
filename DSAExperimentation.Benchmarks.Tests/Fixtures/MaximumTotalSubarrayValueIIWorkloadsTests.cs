using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaximumTotalSubarrayValueIIWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 3691's values being drawn from the problem's full range, so a window's max and min rarely coincide
// and every candidate's value is genuinely data-dependent rather than trivially zero.
public sealed partial class MaximumTotalSubarrayValueIIWorkloadsTests
{
    private const int Size = 256;
    private const int Seed = 3691; // LC problem number
    private const int MinValue = 0;
    private const int ValueUpperBound = 1_000_000_000; // exclusive
    private const int DistinctValueFloor = 2;

    [Fact]
    public void BuildNums_Size_ReturnsOneValuePerPosition() =>
        Assert.Equal(Size, MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysInsideTheHalfOpenRange() =>
        Assert.All(
            MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed),
            value => Assert.InRange(value, MinValue, ValueUpperBound - 1));

    [Fact]
    public void BuildNums_Values_AreNotAllTheSame()
    {
        var nums = MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed);

        Assert.InRange(nums.Distinct().Count(), DistinctValueFloor, Size);
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed),
            MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed));
}
