using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ContainsDuplicateWorkloads (ARCHITECTURE 17.7). The reading depends on
// the array being a permutation of a contiguous range, so both strategies pay their full
// worst case with no duplicate to find early.
public sealed partial class ContainsDuplicateWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 217; // LC problem number

    [Fact]
    public void BuildDistinctValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, ContainsDuplicateWorkloads.BuildDistinctValues(Length, Seed).Length);

    [Fact]
    public void BuildDistinctValues_EveryValue_AppearsExactlyOnce()
    {
        var values = ContainsDuplicateWorkloads.BuildDistinctValues(Length, Seed);

        Assert.Equal(Length, values.Distinct().Count());
    }

    [Fact]
    public void BuildDistinctValues_EveryValue_StaysWithinTheContiguousRange()
    {
        var values = ContainsDuplicateWorkloads.BuildDistinctValues(Length, Seed);

        Assert.All(values, value => Assert.InRange(value, 0, Length - 1));
    }

    [Fact]
    public void BuildDistinctValues_SameSeed_ReturnsTheSameShuffle() =>
        Assert.Equal(
            ContainsDuplicateWorkloads.BuildDistinctValues(Length, Seed),
            ContainsDuplicateWorkloads.BuildDistinctValues(Length, Seed));
}
