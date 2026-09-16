using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TweetCountsPerFrequencyBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the flat list filtered by name on every query
// against the HashMap bucketed by name as tweets are recorded - so a harness whose arms disagree is
// timing two different problems. Both arms sum their per-bucket counts into an int, so they are
// compared directly. Setup seeds both stores with the same TweetCount tweets drawn from one fixed
// seed, so the same TweetCount must rebuild the same workload; each arm queries its own store and
// GetTweetCountsPerFrequency only reads, so one harness can be called twice in either order.
public sealed partial class TweetCountsPerFrequencyBenchmarksTests
{
    private const int SmallestTweetCount = 2_000;

    [Fact]
    public void Setup_SameTweetCount_RebuildsTheSameTweets() =>
        Assert.Equal(
            BuildHarness().FlatListFilterPerQuery(),
            BuildHarness().FlatListFilterPerQuery());

    [Fact]
    public void FlatListFilterPerQuery_SmallestTweetCount_AgreesWithHashMapGroupedByName()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapGroupedByName(), harness.FlatListFilterPerQuery());
    }

    [Fact]
    public void HashMapGroupedByName_SmallestTweetCount_AgreesWithFlatListFilterPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FlatListFilterPerQuery(), harness.HashMapGroupedByName());
    }

    private static TweetCountsPerFrequencyBenchmarks BuildHarness()
    {
        var harness = new TweetCountsPerFrequencyBenchmarks { TweetCount = SmallestTweetCount };
        harness.Setup();

        return harness;
    }
}
