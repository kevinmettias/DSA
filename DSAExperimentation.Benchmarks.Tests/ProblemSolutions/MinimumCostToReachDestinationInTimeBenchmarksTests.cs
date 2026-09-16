using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToReachDestinationInTimeBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the textbook unmemoized walk over every route the
// minute budget can pay for against ShortestPath.Dijkstra run over the (city, elapsedTime) expansion
// of the same map - so a harness whose arms disagree is timing two different maps. Setup builds both
// views from the same step chain and cycling fees, so the same LastCity must rebuild both. An
// unreachable destination is reported as the shared sentinel in both arms.
public sealed partial class MinimumCostToReachDestinationInTimeBenchmarksTests
{
    private const int SmallestLastCity = 10;

    // Both strategies report the same sentinel when the budget cannot pay for any route; the step
    // chain is solvable, so this is only the value a broken workload would collapse onto.
    private const int UnreachableSentinel = -1;

    [Fact]
    public void Setup_SameLastCity_RebuildsTheSameMap() =>
        Assert.Equal(BuildHarness().NaiveDfs(), BuildHarness().NaiveDfs());

    [Fact]
    public void NaiveDfs_TwoWayStepChain_AgreesWithStateExpandedDijkstra()
    {
        var harness = BuildHarness();

        Assert.NotEqual(UnreachableSentinel, harness.NaiveDfs());
        Assert.Equal(harness.StateExpandedDijkstra(), harness.NaiveDfs());
    }

    [Fact]
    public void StateExpandedDijkstra_TwoWayStepChain_AgreesWithNaiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveDfs(), harness.StateExpandedDijkstra());
    }

    private static MinimumCostToReachDestinationInTimeBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToReachDestinationInTimeBenchmarks { LastCity = SmallestLastCity };
        harness.Setup();

        return harness;
    }
}
