using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LargestColorValueInADirectedGraphBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - the largest same-colour count along any
// directed path - so a harness whose arms disagree is timing two different problems. The node
// list and its edges are built in [GlobalSetup] and each arm is handed that prepared graph, so
// the same NodeCount must rebuild the same DAG edge for edge; nothing the arms do mutates the
// node list, so one harness instance is safe to call twice in either order.
public sealed partial class LargestColorValueInADirectedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameColourGraph() =>
        Assert.Equal(
            BuildHarness().RepeatedRelaxation(),
            BuildHarness().RepeatedRelaxation());

    [Fact]
    public void RepeatedRelaxation_SeededAcyclicColourGraph_AgreesWithKahnsTopologicalSortDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KahnsTopologicalSortDp(), harness.RepeatedRelaxation());
    }

    [Fact]
    public void KahnsTopologicalSortDp_SeededAcyclicColourGraph_AgreesWithRepeatedRelaxation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedRelaxation(), harness.KahnsTopologicalSortDp());
    }

    private static LargestColorValueInADirectedGraphBenchmarks BuildHarness()
    {
        var harness = new LargestColorValueInADirectedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
