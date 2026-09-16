using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountVisitedNodesInADirectedGraphBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the per-start forward walk against Tarjan plus a
// reverse BFS - so a harness whose arms disagree is timing two different problems, not two ways of
// answering one. Setup derives the edges from NodeCount alone, so the same NodeCount must rebuild the
// same functional graph; otherwise two published numbers were never comparable in the first place.
//
// The edges and the prepared graph are both private, but the workload's defining property - one
// cycle spanning every node - decides the answer outright: from any start the walk sees the whole
// cycle before it repeats a node, so every entry of every arm's answer is NodeCount.
public sealed partial class CountVisitedNodesInADirectedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameSingleCycle()
    {
        Assert.Equal(EveryStartVisitsEveryNode(SmallestNodeCount), BuildHarness().PerStartWalk());
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerStartWalk()),
            AnswerText.Of(BuildHarness().PerStartWalk()));
    }

    [Fact]
    public void PerStartWalk_SingleCycleSpansEveryNode_AgreesWithSccPlusReverseBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SccPlusReverseBfs()), AnswerText.Of(harness.PerStartWalk()));
    }

    [Fact]
    public void SccPlusReverseBfs_SingleCycleSpansEveryNode_AgreesWithPerStartWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PerStartWalk()), AnswerText.Of(harness.SccPlusReverseBfs()));
    }

    // AnswerText.Of rather than OfUnorderedSet: both arms report one count per start node, so a
    // value's position is the node it belongs to, not an arbitrary outer order.
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
