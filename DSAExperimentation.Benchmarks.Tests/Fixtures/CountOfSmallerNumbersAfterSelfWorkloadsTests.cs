using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CountOfSmallerNumbersAfterSelfWorkloads (ARCHITECTURE 17.7). The fixture
// decides only how many random values to hand the solution, which coordinates and sweeps them
// itself, so what it owes a test is the length, the value band and seed-stable rebuilds.
public sealed partial class CountOfSmallerNumbersAfterSelfWorkloadsTests
{
    private const int Length = 128;
    private const int Seed = 315; // LC problem number
    private const int ValueBound = 10_000;

    [Fact]
    public void BuildNums_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysWithinTheSignedBound()
    {
        var nums = CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, Seed);

        Assert.All(nums, value => Assert.InRange(value, -ValueBound, ValueBound - 1));
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameNums() =>
        Assert.Equal(
            CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, Seed),
            CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, Seed));
}
