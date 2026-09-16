using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignRideSharingSystemBenchmarks's nested AddDriverOperation - the sibling
// of AddRiderOperationTests, which carries the fuller account of why a nested unit needs a class
// named after it and why the seeding pass is only observable through the outer arms.
//
// This class exercises the other tuned size (the benchmark declares [Params(500, 5_000)]), so the
// band below is recomputed for 5,000 rather than borrowed from the 500-rider class. Drivers are
// seeded as [RiderCount, 2 * RiderCount) by the pass under test; a driver pass that seeded riders
// instead - or seeded nothing - would leave every match without a counterpart and the summed id
// total would collapse to zero, below the derived floor.
public sealed partial class AddDriverOperationTests
{
    private const int LargestRiderCount = 5_000;

    private const int CancelledFraction = 10;
    private const int HighestCancelCount = LargestRiderCount / CancelledFraction;
    private const int FewestMatchedPairs = LargestRiderCount - HighestCancelCount;

    private const int LowestDriverId = LargestRiderCount;
    private const int HighestDriverId = (2 * LargestRiderCount) - 1;
    private const int HighestRiderId = LargestRiderCount - 1;
    private const long LowestIdSum = (long)FewestMatchedPairs * LowestDriverId;
    private const long HighestIdSum = (long)LargestRiderCount * (HighestDriverId + HighestRiderId);

    [Fact]
    public void Apply_SeedsEveryDriverAboveTheRiderRange_SoEveryArmStillFindsACounterpart() =>
        Assert.InRange(BuildHarness().LazyDeletionQueue(), LowestIdSum, HighestIdSum);

    // The seeding pass draws its cancelled riders from new Random(3829) inside Setup, so a fresh
    // harness has to reproduce the same script - and therefore the same id total - every time. A
    // driver pass that leaked state between harnesses would break this on the second one.
    [Fact]
    public void Apply_TwoFreshHarnessesAtTheSameSeed_ReplayTheSameIdTotal() =>
        Assert.Equal(BuildHarness().LinearScanQueue(), BuildHarness().LinearScanQueue());

    private static DesignRideSharingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignRideSharingSystemBenchmarks { RiderCount = LargestRiderCount };
        harness.Setup();

        return harness;
    }
}
