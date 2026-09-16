using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarks (ARCHITECTURE 17.9): its two arms are
// AllAncestorsOfANodeInADirectedAcyclicGraphSolution's competing strategies for the same question - one fresh
// forward walk per start node against one Kahn ordering plus a single linear DP pass - so a harness whose arms
// disagree is answering two different graphs. LC 2192 pins both orders the answer can have: one ascending ancestor
// list per node id, in node order. Setup builds the capped-fan-out DAG off the node count alone - no random draw -
// so the same NodeCount must rebuild the same graph.
public sealed partial class AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarksTests
{
    // The smaller of Setup's [Params(50, 1_000)] node counts.
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaivePerNodeForwardWalk()),
            AnswerText.Of(BuildHarness().NaivePerNodeForwardWalk()));

    [Fact]
    public void NaivePerNodeForwardWalk_FiftyNodeDag_AgreesWithTopologicalDpPass()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TopologicalDpPass()),
            AnswerText.Of(harness.NaivePerNodeForwardWalk()));
    }

    [Fact]
    public void TopologicalDpPass_FiftyNodeDag_AgreesWithNaivePerNodeForwardWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.NaivePerNodeForwardWalk()),
            AnswerText.Of(harness.TopologicalDpPass()));
    }

    private static AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarks BuildHarness()
    {
        var harness = new AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
