using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ElevatorRequestsIBenchmarks (ARCHITECTURE 17.9): a bound the workload's shape
// fixes without either arm. Setup pins floorCount to the request count and draws every request below
// that floor count, which is exactly the bound LC 4020 documents: no hop can be longer than
// floorCount - 1, over RequestCount hops.
public sealed partial class ElevatorRequestsIBenchmarksTests
{
    private const int SmallestRequestCount = 10;
    private const int MinimumTravelForRequestsBelowTheFloorCount = 1;
    private const int MaximumTravelForTheSmallestRequestCount = SmallestRequestCount * (SmallestRequestCount - 1);

    [Fact]
    public void InlineAbsoluteDifference_TenRequestsBelowFloorTen_TravelStaysInsideTheFloorBound() =>
        Assert.InRange(
            BuildHarness().InlineAbsoluteDifference(),
            MinimumTravelForRequestsBelowTheFloorCount,
            MaximumTravelForTheSmallestRequestCount);

    private static ElevatorRequestsIBenchmarks BuildHarness()
    {
        var harness = new ElevatorRequestsIBenchmarks { RequestCount = SmallestRequestCount };
        harness.Setup();

        return harness;
    }
}
