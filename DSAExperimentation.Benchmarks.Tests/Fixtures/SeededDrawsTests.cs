using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SeededDraws (ARCHITECTURE 17.7), the draw helper the offline-sweep harnesses
// of LC 2940, LC 1707 and LC 3569 share. The reading depends on the values genuinely partitioning -
// the range has to be far wider than the value count, or a query's bounds would select everything
// rather than a slice - and on both draws coming off ONE stream, because a caller interleaves its
// values and its queries and a fresh Random per call would restart the sequence and change the
// workload a recorded measurement was taken against.
public sealed partial class SeededDrawsTests
{
    private const int Count = 64;
    private const int Seed = 2940; // LC problem number
    private const int LowInclusive = 1;
    private const int HighExclusive = 100_000;
    private const int PairFieldCount = 2; // two independent draws of the same quantity

    [Fact]
    public void Values_Count_ReturnsOneValuePerPosition() =>
        Assert.Equal(
            Count,
            SeededDraws.Values(Count, LowInclusive, HighExclusive, new Random(Seed)).Length);

    [Fact]
    public void Values_EveryValue_FallsInsideTheHalfOpenRange() =>
        Assert.All(
            SeededDraws.Values(Count, LowInclusive, HighExclusive, new Random(Seed)),
            value => Assert.InRange(value, LowInclusive, HighExclusive - 1));

    [Fact]
    public void Values_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            SeededDraws.Values(Count, LowInclusive, HighExclusive, new Random(Seed)),
            SeededDraws.Values(Count, LowInclusive, HighExclusive, new Random(Seed)));

    [Fact]
    public void Pairs_Count_ReturnsOnePairOfDrawsPerPosition()
    {
        var pairs = SeededDraws.Pairs(Count, LowInclusive, HighExclusive, new Random(Seed));

        Assert.Equal(Count, pairs.Length);
        Assert.All(pairs, pair => Assert.Equal(PairFieldCount, pair.Length));
    }

    [Fact]
    public void Pairs_EveryValue_FallsInsideTheHalfOpenRange() =>
        Assert.All(
            SeededDraws.Pairs(Count, LowInclusive, HighExclusive, new Random(Seed))
                .SelectMany(pair => pair),
            value => Assert.InRange(value, LowInclusive, HighExclusive - 1));

    [Fact]
    public void Pairs_SameSeed_ReturnsTheSamePairs() =>
        Assert.Equal(
            AnswerText.Of(SeededDraws.Pairs(Count, LowInclusive, HighExclusive, new Random(Seed))),
            AnswerText.Of(SeededDraws.Pairs(Count, LowInclusive, HighExclusive, new Random(Seed))));

    // The documented reason this helper takes a Random rather than a seed. A helper that rebuilt a
    // fresh Random from a seed inside would pass every test above and still quietly change the
    // interleaved workload, so the stream the caller already advanced must not be restarted.
    [Fact]
    public void Pairs_OnAStreamThatAlreadyProducedTheValues_DoesNotRestartThatStream()
    {
        var interleaved = new Random(Seed);
        SeededDraws.Values(Count, LowInclusive, HighExclusive, interleaved);

        Assert.NotEqual(
            AnswerText.Of(SeededDraws.Pairs(Count, LowInclusive, HighExclusive, interleaved)),
            AnswerText.Of(SeededDraws.Pairs(Count, LowInclusive, HighExclusive, new Random(Seed))));
    }
}
