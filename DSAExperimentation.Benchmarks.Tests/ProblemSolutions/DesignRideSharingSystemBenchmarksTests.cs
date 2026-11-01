using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignRideSharingSystemBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a bound derived from the script
// rather than from either arm: the script has to seed every rider and driver id before it matches
// any of them.
public sealed partial class DesignRideSharingSystemBenchmarksTests
{
    private const int SmallestRiderCount = 32;

    // The script cancels a tenth of the riders before the match rounds run, so at least this many
    // riders are still pending when matching starts - and a match consumes one driver and one
    // rider exactly once each.
    private const int CancelledRiderCount = SmallestRiderCount / 10;

    [Fact]
    public void Apply_SeededRiderAndDriverOperations_RegisterEveryDistinctId()
    {
        // The two seeding passes are what the match rounds draw on, and each is a single-id call
        // per id - so the pairs they leave behind are distinct driver ids and distinct rider ids
        // from the seeded ranges, matched at most once each. A seeding pass whose ids collapsed to
        // one value would leave a single pair to match and a sum near zero instead.
        Assert.InRange(BuildHarness().LinearScanQueue(), MinimumMatchedIdSum(), MaximumMatchedIdSum());
    }

    private static DesignRideSharingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignRideSharingSystemBenchmarks { RiderCount = SmallestRiderCount };
        harness.Setup();

        return harness;
    }

    // At least SmallestRiderCount - CancelledRiderCount pairs match, and their ids sum to the least when
    // they are the smallest of their ranges.
    private static long MinimumMatchedIdSum() => IdSumOfTheFirstPairs(SmallestRiderCount - CancelledRiderCount);

    // A loose ceiling: every rider matches, so both whole id ranges are summed.
    private static long MaximumMatchedIdSum() => IdSumOfTheFirstPairs(SmallestRiderCount);

    // Riders are numbered from 1 and drivers from SmallestRiderCount + 1, so the first n of each sum to
    // 1 + ... + n and (R + 1) + ... + (R + n): together n * R + n * (n + 1).
    private static long IdSumOfTheFirstPairs(long pairs) => (pairs * SmallestRiderCount) + (pairs * (pairs + 1));
}
