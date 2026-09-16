using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaximumGapWorkloads (ARCHITECTURE 17.7). The reading depends on LC 164's own
// methods copying and sorting a random value list, so all this owes is the number of values and their
// range.
public sealed partial class MaximumGapWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 164; // LC problem number
    private const int MinValue = 0;
    private const int ValueExclusiveBound = 1_000_000;
    private const int DistinctValueFloor = 2;

    [Fact]
    public void BuildValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, MaximumGapWorkloads.BuildValues(Length, Seed).Length);

    [Fact]
    public void BuildValues_EveryValue_StaysInsideTheHalfOpenRange() =>
        Assert.All(
            MaximumGapWorkloads.BuildValues(Length, Seed),
            value => Assert.InRange(value, MinValue, ValueExclusiveBound - 1));

    // A run of equal values would sort to a zero gap everywhere, which is the degenerate case the
    // two arms' maximum-gap scan is not meant to be measured on.
    [Fact]
    public void BuildValues_Values_AreNotAllTheSame()
    {
        var values = MaximumGapWorkloads.BuildValues(Length, Seed);

        Assert.InRange(values.Distinct().Count(), DistinctValueFloor, Length);
    }

    [Fact]
    public void BuildValues_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            MaximumGapWorkloads.BuildValues(Length, Seed),
            MaximumGapWorkloads.BuildValues(Length, Seed));
}
