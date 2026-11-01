using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAuctionSystemBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a ceiling derived from the call
// script rather than from either arm.
public sealed partial class DesignAuctionSystemBenchmarksTests
{
    private const int SmallestInitialBidCount = 200;

    // Every query reports a bidder id the script itself created - the lowest of which is LC 3815's
    // smallest id, one, and none of which passes three times the seeded count - or reports no bidder at
    // all, which the replay counts as zero.
    private const long MinimumBidderIdSum = 0;

    private const long MaximumBidderIdSum = 3L * SmallestInitialBidCount * SmallestInitialBidCount;

    // The growth rounds add two fresh bids before every query, so every query has a bidder to find.
    // Every userId the script ever creates is at most three times the seeded count - the seeded range,
    // then two more per round - which is the ceiling the summed report of the InitialBidCount queries
    // has to stay within.
    [Fact]
    public void LinearScan_SeededBidScript_SumsOnlyBidderIdsTheScriptCreated() =>
        Assert.InRange(BuildHarness().LinearScan(), MinimumBidderIdSum, MaximumBidderIdSum);

    private static DesignAuctionSystemBenchmarks BuildHarness()
    {
        var harness = new DesignAuctionSystemBenchmarks { InitialBidCount = SmallestInitialBidCount };
        harness.Setup();

        return harness;
    }
}
