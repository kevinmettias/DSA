using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignTwitterBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: which
// tweets the feed holds, known from Setup's construction rather than from either arm. Setup builds the whole call
// script from one fixed seed, and each arm returns the feed the replay reads at the end.
public sealed partial class DesignTwitterBenchmarksTests
{
    private const int SmallestFollowedUsers = 50;

    // Mirrors the benchmark's own tweets-per-source: the script posts this many tweets for every
    // followed source, and the replay reads the feed of the user following all of them.
    private const int TweetsPerSource = 20;

    // LeetCode caps getNewsFeed at ten tweets, newest first.
    private const int FeedSize = 10;

    private const int TotalTweetCount = SmallestFollowedUsers * TweetsPerSource;

    [Fact]
    public void GatherAllAndSort_InterleavedSourceScript_ReadsTheNewestTweetsNewestFirst() =>
        Assert.Equal(NewestTweetIds(), BuildHarness().GatherAllAndSort());

    [Fact]
    public void HeapKWayMerge_InterleavedSourceScript_ReadsTheNewestTweetsNewestFirst() =>
        Assert.Equal(NewestTweetIds(), BuildHarness().HeapKWayMerge());

    // Tweet ids are posted in increasing order, one per source drawn at random, so the newest
    // FeedSize of them are the ids the script posted last whatever order the sources came out in.
    // A feed that gathered only one source, or that sorted the wrong way, holds different ids.
    private static IEnumerable<int> NewestTweetIds() =>
        Enumerable.Range(TotalTweetCount - FeedSize, FeedSize).Reverse();

    private static DesignTwitterBenchmarks BuildHarness()
    {
        var harness = new DesignTwitterBenchmarks { FollowedUsers = SmallestFollowedUsers };
        harness.Setup();

        return harness;
    }
}
