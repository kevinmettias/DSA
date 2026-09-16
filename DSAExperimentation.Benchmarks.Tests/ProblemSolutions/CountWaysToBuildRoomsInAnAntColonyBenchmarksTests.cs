using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToBuildRoomsInAnAntColonyBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - recomputing Factorial(subtree size) at every node
// against a table prepared once - so a harness whose arms disagree is timing two different problems,
// not two ways of answering one. Setup builds the tree from RoomCount alone, so the same RoomCount
// must rebuild the same tree; otherwise two published numbers were never comparable in the first
// place.
//
// The tree is private, and the workload's defining property - a straight chain, the deepest tree a
// given room count admits - decides the answer outright: a chain has exactly one build order, the
// chain itself, so a room count that produced any other shape would not answer 1.
public sealed partial class CountWaysToBuildRoomsInAnAntColonyBenchmarksTests
{
    private const int SmallestRoomCount = 200;

    private const int ExpectedBuildOrders = 1;

    [Fact]
    public void Setup_SameRoomCount_RebuildsTheSameChain()
    {
        Assert.Equal(ExpectedBuildOrders, BuildHarness().PerNodeFactorials());
        Assert.Equal(BuildHarness().PerNodeFactorials(), BuildHarness().PerNodeFactorials());
    }

    [Fact]
    public void PerNodeFactorials_StraightChain_AgreesWithPrecomputedFactorials()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedFactorials(), harness.PerNodeFactorials());
    }

    [Fact]
    public void PrecomputedFactorials_StraightChain_AgreesWithPerNodeFactorials()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerNodeFactorials(), harness.PrecomputedFactorials());
    }

    private static CountWaysToBuildRoomsInAnAntColonyBenchmarks BuildHarness()
    {
        var harness = new CountWaysToBuildRoomsInAnAntColonyBenchmarks { RoomCount = SmallestRoomCount };
        harness.Setup();

        return harness;
    }
}
