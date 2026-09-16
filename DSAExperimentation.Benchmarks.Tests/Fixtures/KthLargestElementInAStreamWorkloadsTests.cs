using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for KthLargestElementInAStreamWorkloads (ARCHITECTURE 17.7). The reading depends on
// the stream being one fixed sequence of Add values, so both LC 703 strategies process the same
// interleaved insert requests.
public sealed partial class KthLargestElementInAStreamWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 703; // LC problem number
    private const int MaxValueExclusive = 1_000_000;
    private const int MinValue = 1;

    [Fact]
    public void BuildStream_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(
            Length,
            KthLargestElementInAStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive).Length);

    [Fact]
    public void BuildStream_EveryValue_StaysInsideTheHalfOpenRange()
    {
        var stream = KthLargestElementInAStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive);

        Assert.All(stream, value => Assert.InRange(value, MinValue, MaxValueExclusive - 1));
    }

    [Fact]
    public void BuildStream_SameSeed_ReturnsTheSameStream() =>
        Assert.Equal(
            KthLargestElementInAStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive),
            KthLargestElementInAStreamWorkloads.BuildStream(Length, Seed, MaxValueExclusive));
}
