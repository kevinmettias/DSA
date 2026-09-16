using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignRideSharingSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a queue that skips cancelled riders by rebuilding
// against one that skips them by lazily discarding popped entries - so a harness whose arms
// disagree is timing two different problems. Setup builds the whole call script from one fixed
// seed, so the same RiderCount must rebuild the same script, and that script has to seed every
// rider and driver id before it matches any of them.
public sealed partial class DesignRideSharingSystemBenchmarksTests
{
    private const int SmallestRiderCount = 500;

    // The script cancels a tenth of the riders before the match rounds run, so at least this many
    // riders are still pending when matching starts - and a match consumes one driver and one
    // rider exactly once each.
    private const int CancelledRiderCount = SmallestRiderCount / 10;

    [Fact]
    public void Setup_SameRiderCount_RebuildsTheSameCallScript()
    {
        // Every match reports the pair it consumed, and the summed report is bounded below by the
        // smallest total two sets of that many distinct ids can have - which the match rounds only
        // reach if the seeding script put every one of those ids in.
        Assert.InRange(BuildHarness().LinearScanQueue(), MinimumMatchedIdSum(), MaximumMatchedIdSum());
        Assert.Equal(BuildHarness().LinearScanQueue(), BuildHarness().LinearScanQueue());
    }

    [Fact]
    public void Apply_SeededRiderAndDriverOperations_RegisterEveryDistinctId()
    {
        // The two seeding passes are what the match rounds draw on, and each is a single-id call
        // per id - so the pairs they leave behind are distinct driver ids and distinct rider ids
        // from the seeded ranges, matched at most once each. A seeding pass whose ids collapsed to
        // one value would leave a single pair to match and a sum near zero instead.
        Assert.InRange(BuildHarness().LinearScanQueue(), MinimumMatchedIdSum(), MaximumMatchedIdSum());
    }

    [Fact]
    public void LinearScanQueue_CancelledRiderScript_AgreesWithLazyDeletionQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionQueue(), harness.LinearScanQueue());
    }

    [Fact]
    public void LazyDeletionQueue_CancelledRiderScript_AgreesWithLinearScanQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanQueue(), harness.LazyDeletionQueue());
    }

    private static DesignRideSharingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignRideSharingSystemBenchmarks { RiderCount = SmallestRiderCount };
        harness.Setup();

        return harness;
    }

    // The id sum of two sets of n distinct ids drawn from 0..SmallestRiderCount-1 is smallest when
    // both sets are the n smallest ids - 0 + 1 + ... + (n - 1), twice over - and largest when both
    // are the n largest.
    private static long MinimumMatchedIdSum()
    {
        var matchedPairs = SmallestRiderCount - CancelledRiderCount;

        return (long)matchedPairs * (matchedPairs - 1);
    }

    // A loose ceiling: at most every rider matches, and the two matched id sets then hold
    // distinct ids no larger than SmallestRiderCount - 1 each.
    private static long MaximumMatchedIdSum() =>
        2L * SmallestRiderCount * (SmallestRiderCount - 1);
}
