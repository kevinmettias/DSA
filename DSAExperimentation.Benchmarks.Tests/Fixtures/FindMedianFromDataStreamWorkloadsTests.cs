using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FindMedianFromDataStreamWorkloads (ARCHITECTURE 17.7). The reading depends
// on the stream being one fixed sequence of AddNum values, so both LC 295 strategies process the
// same interleaved AddNum/FindMedian calls.
public sealed partial class FindMedianFromDataStreamWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 295; // LC problem number
    private const int MaxValueExclusive = 1_000_000;
    private const int MinValue = 1;

    [Fact]
    public void BuildStream_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(
            Length,
            FindMedianFromDataStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive).Length);

    [Fact]
    public void BuildStream_EveryValue_StaysInsideTheHalfOpenRange()
    {
        var stream = FindMedianFromDataStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive);

        Assert.All(stream, value => Assert.InRange(value, MinValue, MaxValueExclusive - 1));
    }

    [Fact]
    public void BuildStream_SameSeed_ReturnsTheSameStream() =>
        Assert.Equal(
            FindMedianFromDataStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive),
            FindMedianFromDataStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive));
}
