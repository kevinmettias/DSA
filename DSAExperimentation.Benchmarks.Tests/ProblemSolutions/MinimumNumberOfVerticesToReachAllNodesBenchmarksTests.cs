using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfVerticesToReachAllNodesBenchmarks (ARCHITECTURE 17.9):
// both arms are MinimumNumberOfVerticesToReachAllNodesSolution's, the same methods
// MinimumNumberOfVerticesToReachAllNodesTests proves correct, and both return the same minimal
// set of source vertices - they differ only in how "does this node have an incoming edge" is
// answered - so arms that disagree are timing two different problems.
//
// Both arms collect their answer by scanning node ids in ascending order, so the returned list's
// order is fixed by the implementation rather than left open by the problem, and the
// order-sensitive comparison below is the stricter of the two available renderings.
public sealed partial class MinimumNumberOfVerticesToReachAllNodesBenchmarksTests
{
    // The smallest declared [Params] value: the nested scan is O(nodes * edges), so a smaller
    // graph is the cheaper way to reach the same source-set comparison.
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().NestedScanForZeroInDegree()),
            AnswerText.Of(BuildHarness().NestedScanForZeroInDegree()));

    [Fact]
    public void NestedScanForZeroInDegree_AgreesWithSetTrackedInDegree()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SetTrackedInDegree()),
            AnswerText.Of(harness.NestedScanForZeroInDegree()));
    }

    [Fact]
    public void SetTrackedInDegree_AgreesWithNestedScanForZeroInDegree()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.NestedScanForZeroInDegree()),
            AnswerText.Of(harness.SetTrackedInDegree()));
    }

    private static MinimumNumberOfVerticesToReachAllNodesBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfVerticesToReachAllNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
