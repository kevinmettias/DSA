using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAuctionSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a full linear scan for the highest bid against
// this repo's own lazy-deletion heap - so a harness whose arms disagree is timing two different
// problems. Setup builds the whole call script from one fixed seed, so the same InitialBidCount
// must rebuild the same script, and that script has to keep a live bidder in front of every one
// of its GetHighestBidder calls for either arm to report anything.
public sealed partial class DesignAuctionSystemBenchmarksTests
{
    private const int SmallestInitialBidCount = 200;

    // Every query reports a bidder id the script itself created - the lowest of which is zero, and
    // none of which reaches three times the seeded count - or reports no bidder at all, which the
    // replay counts as zero.
    private const long MinimumBidderIdSum = 0;

    private const long MaximumBidderIdSum = (3L * SmallestInitialBidCount * SmallestInitialBidCount) - 1;

    [Fact]
    public void Setup_SameInitialBidCount_RebuildsTheSameCallScript()
    {
        // The growth rounds add two fresh bids before every query, so every query has a bidder to
        // find. Every userId the script ever creates is below three times the seeded count - the
        // seeded range, then two more per round - which is the ceiling the summed report of the
        // InitialBidCount queries has to stay under.
        Assert.InRange(BuildHarness().LinearScan(), MinimumBidderIdSum, MaximumBidderIdSum);
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_SeededBidScript_AgreesWithLazyDeletionHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionHeap(), harness.LinearScan());
    }

    [Fact]
    public void LazyDeletionHeap_SeededBidScript_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.LazyDeletionHeap());
    }

    private static DesignAuctionSystemBenchmarks BuildHarness()
    {
        var harness = new DesignAuctionSystemBenchmarks { InitialBidCount = SmallestInitialBidCount };
        harness.Setup();

        return harness;
    }
}
