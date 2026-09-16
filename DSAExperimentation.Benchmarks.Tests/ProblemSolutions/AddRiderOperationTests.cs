using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignRideSharingSystemBenchmarks's nested AddRiderOperation.
//
// AddRiderOperation is a private nested type, so no test class can name it in code. Coverage is
// attributed by TYPE, so the nested unit needs a class named after it, and the only surface that
// runs AddRiderOperation.Apply is the outer benchmark's setup script, which every arm replays.
// These tests therefore drive the arms and assert what the seeding pass is responsible for.
//
// What that pass is responsible for: riders are seeded under ids [0, RiderCount) and drivers under
// [RiderCount, 2 * RiderCount), and a match only succeeds when both are pending. So a rider id that
// landed in the driver range - or a seed call that never happened - leaves nothing to match and the
// summed id total collapses to zero. The band below is derived from the seeding ranges and the
// cancel count alone, never from an observed return value, so an arm that stopped matching cannot
// satisfy it. Honest caveat: the arm's total is not specific to THIS operation - it also falls to
// zero if the driver pass breaks - because neither seeding pass is independently observable through
// the public surface. The mutation proof for this class is recorded in the campaign report.
public sealed partial class AddRiderOperationTests
{
    private const int SmallestRiderCount = 500;

    // AppendCancelledRiders draws Math.Max(1, riderCount / CancelledFraction) times from [0,
    // riderCount), so at most that many distinct riders are ever taken out of play.
    private const int CancelledFraction = 10;
    private const int HighestCancelCount = SmallestRiderCount / CancelledFraction;
    private const int FewestMatchedPairs = SmallestRiderCount - HighestCancelCount;

    // Drivers are seeded as [RiderCount, 2 * RiderCount) and riders as [0, RiderCount), so every
    // matched pair contributes at least the lowest driver id, and at most the two range maxima.
    private const int LowestDriverId = SmallestRiderCount;
    private const int HighestDriverId = (2 * SmallestRiderCount) - 1;
    private const int HighestRiderId = SmallestRiderCount - 1;
    private const long LowestIdSum = (long)FewestMatchedPairs * LowestDriverId;
    private const long HighestIdSum = (long)SmallestRiderCount * (HighestDriverId + HighestRiderId);

    [Fact]
    public void Apply_SeedsEveryRiderInTheRiderIdRange_SoBothArmsStillPairThemWithDrivers()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.LinearScanQueue(), LowestIdSum, HighestIdSum);
        Assert.InRange(harness.LazyDeletionQueue(), LowestIdSum, HighestIdSum);
    }

    // The two arms share the harness's script but build their own strategy inside the call, and the
    // script only reads on replay, so one harness answers both calls in either order.
    private static DesignRideSharingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignRideSharingSystemBenchmarks { RiderCount = SmallestRiderCount };
        harness.Setup();

        return harness;
    }
}
