using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToVisitDisappearingNodesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the BCL priority queue's lazy deletion against this
// repo's own Heap - so a harness whose arms disagree is timing two different problems. Both arms only
// read the prebuilt TimedAdjacency and the disappearance times drawn in [GlobalSetup], so one harness
// instance is safe to call twice in either order. Setup draws that workload from one fixed seed, so the
// same NodeCount must rebuild the same graph; otherwise two published numbers were never comparable.
public sealed partial class MinimumTimeToVisitDisappearingNodesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DijkstraQueue()),
            AnswerText.Of(BuildHarness().DijkstraQueue()));

    [Fact]
    public void DijkstraQueue_SeededDisappearingNodes_AgreesWithPriorityHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PriorityHeap()),
            AnswerText.Of(harness.DijkstraQueue()));
    }

    [Fact]
    public void PriorityHeap_SeededDisappearingNodes_AgreesWithDijkstraQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DijkstraQueue()),
            AnswerText.Of(harness.PriorityHeap()));
    }

    private static MinimumTimeToVisitDisappearingNodesBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToVisitDisappearingNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
