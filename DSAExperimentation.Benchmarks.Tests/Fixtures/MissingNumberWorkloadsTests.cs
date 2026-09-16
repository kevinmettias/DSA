using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MissingNumberWorkloads (ARCHITECTURE 17.7). The reading depends on LC 268's array
// being the whole 0..length range with exactly one value taken out and the rest shuffled, so the brute
// force arm cannot exploit an already-sorted input.
public sealed partial class MissingNumberWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 268; // LC problem number
    private const int LowestValue = 0;
    private const int SecondPosition = 1;

    [Fact]
    public void BuildValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, MissingNumberWorkloads.BuildValues(Length, Seed).Length);

    [Fact]
    public void BuildValues_EveryValue_StaysWithinTheClosedRange() =>
        Assert.All(
            MissingNumberWorkloads.BuildValues(Length, Seed),
            value => Assert.InRange(value, LowestValue, Length));

    [Fact]
    public void BuildValues_ExactlyOneValueOfTheRange_IsMissing()
    {
        var values = MissingNumberWorkloads.BuildValues(Length, Seed);

        Assert.Equal(Length, values.Distinct().Count());
        Assert.Single(Enumerable.Range(LowestValue, Length + 1).Except(values));
    }

    [Fact]
    public void BuildValues_Values_AreNotLeftInAscendingOrder()
    {
        var values = MissingNumberWorkloads.BuildValues(Length, Seed);

        Assert.Contains(
            Enumerable.Range(SecondPosition, Length - SecondPosition),
            position => values[position] < values[position - 1]);
    }

    [Fact]
    public void BuildValues_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            MissingNumberWorkloads.BuildValues(Length, Seed),
            MissingNumberWorkloads.BuildValues(Length, Seed));
}
