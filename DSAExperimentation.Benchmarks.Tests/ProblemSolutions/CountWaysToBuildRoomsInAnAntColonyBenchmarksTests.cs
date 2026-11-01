using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToBuildRoomsInAnAntColonyBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// an expected count derived from the workload's construction rather than from any arm.
//
// The tree is private, and the workload's defining property - a straight chain, the deepest tree a
// given room count admits - decides the answer outright: a chain has exactly one build order, the
// chain itself, so a room count that produced any other shape would not answer 1.
public sealed partial class CountWaysToBuildRoomsInAnAntColonyBenchmarksTests
{
    private const int SmallestRoomCount = 200;

    private const int ExpectedBuildOrders = 1;

    [Fact]
    public void PerNodeFactorials_StraightChain_FindsTheOneBuildOrder() =>
        Assert.Equal(ExpectedBuildOrders, BuildHarness().PerNodeFactorials());

    private static CountWaysToBuildRoomsInAnAntColonyBenchmarks BuildHarness()
    {
        var harness = new CountWaysToBuildRoomsInAnAntColonyBenchmarks { RoomCount = SmallestRoomCount };
        harness.Setup();

        return harness;
    }
}
