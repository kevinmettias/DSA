using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for StabilityFactorWorkloads (ARCHITECTURE 17.7). LC 3605's reading depends on
// the array being built as runs of small-prime multiples with occasional guaranteed breaks: the
// runs are what give both the greedy sweep and the binary search real stable subarrays to cut, and
// a bare 1 is coprime to everything, so it ends the run it lands in.
public sealed partial class StabilityFactorWorkloadsTests
{
    private const int Length = 200;
    private const int Seed = 3605; // LC problem number
    private const int BreakValue = 1;
    private const int MinMultiplier = 1;
    private const int MaxMultiplier = 99; // one below the fixture's own exclusive ceiling of 100
    private static readonly int[] SmallPrimes = [2, 3, 5, 7];

    [Fact]
    public void Build_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, StabilityFactorWorkloads.Build(Length, Seed).Length);

    // Every value is either the break value or a small prime times a multiplier inside the band, so
    // no value carries a factor the sieve would have to reach past its own small-prime set for.
    [Fact]
    public void Build_EveryValue_IsEitherABreakValueOrASmallPrimeMultiple() =>
        Assert.All(
            StabilityFactorWorkloads.Build(Length, Seed),
            value => Assert.True(value == BreakValue || IsSmallPrimeMultiple(value)));

    [Fact]
    public void Build_Array_ContainsBreakValuesThatCutTheStableRunsApart()
    {
        var nums = StabilityFactorWorkloads.Build(Length, Seed);

        Assert.Contains(nums, value => value == BreakValue);
        Assert.Contains(nums, value => value != BreakValue);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameArray() =>
        Assert.Equal(
            StabilityFactorWorkloads.Build(Length, Seed),
            StabilityFactorWorkloads.Build(Length, Seed));

    private static bool IsSmallPrimeMultiple(int value) =>
        SmallPrimes.Any(prime => value % prime == 0
            && value / prime >= MinMultiplier
            && value / prime <= MaxMultiplier);
}
