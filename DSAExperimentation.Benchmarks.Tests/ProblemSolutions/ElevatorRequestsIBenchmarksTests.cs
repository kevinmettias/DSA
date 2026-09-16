using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ElevatorRequestsIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - adding Math.Abs of every hop by hand against reading the same
// hop's distance from this repo's own ManhattanHeuristic on a one-column grid - so a harness whose
// arms disagree is timing two different problems. Setup pins floorCount to the request count and
// draws every request below that floor count, which is exactly the bound LC 4020 documents: no hop
// can be longer than floorCount - 1, over RequestCount hops. The requests come from one fixed seed,
// so the same RequestCount must rebuild the same walk.
public sealed partial class ElevatorRequestsIBenchmarksTests
{
    private const int SmallestRequestCount = 100;
    private const int MinimumTravelForRequestsBelowTheFloorCount = 1;
    private const int MaximumTravelForTheSmallestRequestCount = SmallestRequestCount * (SmallestRequestCount - 1);

    [Fact]
    public void Setup_HundredRequestsBelowFloorOneHundred_TravelStaysInsideTheFloorBoundAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var total = harness.InlineAbsoluteDifference();

        Assert.InRange(total, MinimumTravelForRequestsBelowTheFloorCount, MaximumTravelForTheSmallestRequestCount);
        Assert.Equal(total, BuildHarness().InlineAbsoluteDifference());
    }

    [Fact]
    public void InlineAbsoluteDifference_SeededRequestWalk_AgreesWithManhattanHeuristicSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManhattanHeuristicSum(), harness.InlineAbsoluteDifference());
    }

    [Fact]
    public void ManhattanHeuristicSum_SeededRequestWalk_AgreesWithInlineAbsoluteDifference()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InlineAbsoluteDifference(), harness.ManhattanHeuristicSum());
    }

    private static ElevatorRequestsIBenchmarks BuildHarness()
    {
        var harness = new ElevatorRequestsIBenchmarks { RequestCount = SmallestRequestCount };
        harness.Setup();

        return harness;
    }
}
