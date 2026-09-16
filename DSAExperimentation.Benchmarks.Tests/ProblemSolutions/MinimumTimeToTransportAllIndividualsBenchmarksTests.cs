using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToTransportAllIndividualsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - walking the on-the-fly priority queue over
// plain time/mul arrays against Dijkstra over the TransportGraph those same arrays were hoisted into
// in [GlobalSetup] - so a harness whose arms disagree is timing two different problems. Both arms
// only read the prepared graph and the seeded arrays, so one harness instance is safe to call twice
// in either order. Setup draws the workload from one fixed seed, so the same IndividualCount must
// rebuild the same stage multipliers; otherwise two published numbers were never comparable.
//
// Both arms return the minimum time as a double, so the two are compared under a named relative
// tolerance rather than for bit equality: both walk the same optimum, but one pays it through a
// priority queue of scalars and the other through graph edges whose products are accumulated in a
// different order, so the two are free to reach it through different float arithmetic.
public sealed partial class MinimumTimeToTransportAllIndividualsBenchmarksTests
{
    private const int SmallestIndividualCount = 6;

    private const double RelativeTolerance = 1e-9;

    // Transport times are at least the smallest stage multiplier, but a degenerate fixture could
    // return 0; the floor keeps the tolerance band a real number rather than a bare zero.
    private const double TimeScaleFloor = 1.0;

    [Fact]
    public void Setup_SameIndividualCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForceDijkstra(),
            BuildHarness().BruteForceDijkstra(),
            RelativeTolerance * TimeScaleFloor);

    [Fact]
    public void BruteForceDijkstra_SeededStageWorkload_AgreesWithDijkstraOverTransportGraph()
    {
        var harness = BuildHarness();
        var bruteForce = harness.BruteForceDijkstra();

        Assert.Equal(
            bruteForce,
            harness.DijkstraOverTransportGraph(),
            RelativeTolerance * Math.Max(bruteForce, TimeScaleFloor));
    }

    [Fact]
    public void DijkstraOverTransportGraph_SeededStageWorkload_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();
        var graphWalk = harness.DijkstraOverTransportGraph();

        Assert.Equal(
            graphWalk,
            harness.BruteForceDijkstra(),
            RelativeTolerance * Math.Max(graphWalk, TimeScaleFloor));
    }

    private static MinimumTimeToTransportAllIndividualsBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToTransportAllIndividualsBenchmarks { IndividualCount = SmallestIndividualCount };
        harness.Setup();

        return harness;
    }
}
