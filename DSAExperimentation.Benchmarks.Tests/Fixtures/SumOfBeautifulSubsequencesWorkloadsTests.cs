using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SumOfBeautifulSubsequencesWorkloads (ARCHITECTURE 17.7). The LC 3671 reading
// is built at whatever length an arm asks for - up to 16 for the brute-force arm's 2^n subsequence
// enumeration, on to 10^4 for the divisor-sieve arm - so what this fixture has to hold is values in
// bound and varied enough to give the sieve real divisors, checked here at the brute force's largest.
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
