using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NonDecreasingSubarrayWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3420's values staying inside this file's own tighter range, so subarrays land on both sides of the
// budget and the sliding window actually grows and shrinks instead of resetting immediately.
public sealed partial class NonDecreasingSubarrayWorkloadsTests
{
    private const int Size = 256;
    private const int Seed = 3420; // LC problem number
    private const int MinValue = 1;
    private const int ValueUpperBound = 1000; // exclusive
    private const int DistinctValueFloor = 2;

    [Fact]
    public void BuildNums_Size_ReturnsOneValuePerPosition() =>
        Assert.Equal(Size, NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysInsideTheHalfOpenRange() =>
        Assert.All(
            NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed),
            value => Assert.InRange(value, MinValue, ValueUpperBound - 1));

    [Fact]
    public void BuildNums_Values_AreNotAllTheSame()
    {
        var nums = NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed);

        Assert.InRange(nums.Distinct().Count(), DistinctValueFloor, Size);
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed),
            NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed));
}
