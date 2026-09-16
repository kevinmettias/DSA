using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignTwitterBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - concatenating every followed source's history and sorting it
// against a k-way merge of the same histories - so a harness whose arms disagree is timing two
// different problems. Setup builds the whole call script from one fixed seed, so the same
// FollowedUsers must rebuild the same script, and that script has to leave a full feed behind.
public sealed partial class DesignTwitterBenchmarksTests
{
    private const int SmallestFollowedUsers = 50;

    // Mirrors the benchmark's own tweets-per-source: the script posts this many tweets for every
    // followed source, and the replay reads the feed of the user following all of them.
    private const int TweetsPerSource = 20;

    // LeetCode caps getNewsFeed at ten tweets, newest first.
    private const int FeedSize = 10;

    private const int TotalTweetCount = SmallestFollowedUsers * TweetsPerSource;

    // The feed holds the last FeedSize tweet ids posted, so its sum is FeedSize * TotalTweetCount
    // less the ids 0 through FeedSize - 1 those newest tweets displace.
    private const int ExpectedFeedSum = (FeedSize * TotalTweetCount) - ((FeedSize * (FeedSize + 1)) / 2);

    [Fact]
    public void Setup_SameFollowedUsers_RebuildsTheSameFollowAndTweetScript()
    {
        // Tweet ids are posted in increasing order, one per source drawn at random, so the newest
        // FeedSize of them are the ids the script posted last whatever order the sources came out
        // in. A feed that gathered only one source, or that sorted the wrong way, reports a
        // different sum than the whole script's own newest ids do.
        Assert.Equal(ExpectedFeedSum, BuildHarness().GatherAllAndSort());
        Assert.Equal(BuildHarness().GatherAllAndSort(), BuildHarness().GatherAllAndSort());
    }

    [Fact]
    public void GatherAllAndSort_InterleavedSourceScript_AgreesWithHeapKWayMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapKWayMerge(), harness.GatherAllAndSort());
    }

    [Fact]
    public void HeapKWayMerge_InterleavedSourceScript_AgreesWithGatherAllAndSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GatherAllAndSort(), harness.HeapKWayMerge());
    }

    private static DesignTwitterBenchmarks BuildHarness()
    {
        var harness = new DesignTwitterBenchmarks { FollowedUsers = SmallestFollowedUsers };
        harness.Setup();

        return harness;
    }
}
