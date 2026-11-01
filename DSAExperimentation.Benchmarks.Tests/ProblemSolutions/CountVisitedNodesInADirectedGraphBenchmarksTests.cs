using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountVisitedNodesInADirectedGraphBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// an expected answer derived from the workload's construction rather than from either arm.
//
// The edges and the prepared graph are both private, but the workload's defining property - one
// cycle spanning every node - decides the answer outright: from any start the walk sees the whole
// cycle before it repeats a node, so every entry of the answer is NodeCount.
public sealed partial class CountVisitedNodesInADirectedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void PerStartWalk_SingleCycleSpansEveryNode_VisitsEveryNodeFromEveryStart() =>
        Assert.Equal(EveryStartVisitsEveryNode(SmallestNodeCount), BuildHarness().PerStartWalk());

    private static CountVisitedNodesInADirectedGraphBenchmarks BuildHarness()
    {
        var harness = new CountVisitedNodesInADirectedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static int[] EveryStartVisitsEveryNode(int nodeCount)
    {
        var visitedByStart = new int[nodeCount];
        Array.Fill(visitedByStart, nodeCount);

        return visitedByStart;
    }
}
