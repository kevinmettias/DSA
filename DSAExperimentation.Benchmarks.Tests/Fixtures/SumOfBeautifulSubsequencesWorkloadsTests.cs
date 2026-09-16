using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SumOfBeautifulSubsequencesWorkloads (ARCHITECTURE 17.7). The LC 3671 reading
// is kept small enough that the brute-force arm's 2^n subsequence enumeration still finishes an
// iteration, which is the one property this fixture has to hold - the divisor-sieve arm's own
// asymptotic edge only shows up at sizes a bitmask baseline could never join it at.
public sealed partial class SumOfBeautifulSubsequencesWorkloadsTests
{
    private const int Size = 16;
    private const int Seed = 3671; // LC problem number
    private const int MinValue = 1;
    private const int MaxValue = 1_000;
    private const int FewestDistinctValues = 1;

    [Fact]
    public void BuildNums_Size_ReturnsOneValuePerPosition() =>
        Assert.Equal(Size, SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed).Length);

    [Fact]
    public void BuildNums_EveryValue_StaysInsideTheDocumentedBound() =>
        Assert.All(
            SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed),
            value => Assert.InRange(value, MinValue, MaxValue));

    [Fact]
    public void BuildNums_Values_VarySoTheDivisorSieveHasRealDivisorsToScore() =>
        Assert.True(
            SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed).Distinct().Count() > FewestDistinctValues);

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed),
            SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed));
}
