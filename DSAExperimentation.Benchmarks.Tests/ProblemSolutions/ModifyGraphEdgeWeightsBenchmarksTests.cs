using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ModifyGraphEdgeWeightsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - one Dijkstra pass per candidate weight against pricing the -1 edge
// from its two settled half-distances - so a harness whose arms disagree is timing two different problems.
// The mutable subject is not the hoisted field: each arm rebuilds its own weight-assignable graph from the
// raw _edges array inside the call and only writes into that private copy, so the field itself is never
// changed and one harness instance is safe to call twice in either order. Setup derives the stretched
// chain from the [Params] StretchAmount alone, so the same value must rebuild the same edges and target.
public sealed partial class ModifyGraphEdgeWeightsBenchmarksTests
{
    private const int SmallestStretchAmount = 50;

    [Fact]
    public void Setup_SameStretchAmount_RebuildsTheSameChainAndTarget() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanFromOne()),
            AnswerText.Of(BuildHarness().LinearScanFromOne()));

    [Fact]
    public void LinearScanFromOne_SingleAssignableEdgeChain_AgreesWithTwoPassFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TwoPassFormula()),
            AnswerText.Of(harness.LinearScanFromOne()));
    }

    [Fact]
    public void TwoPassFormula_SingleAssignableEdgeChain_AgreesWithLinearScanFromOne()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanFromOne()),
            AnswerText.Of(harness.TwoPassFormula()));
    }

    private static ModifyGraphEdgeWeightsBenchmarks BuildHarness()
    {
        var harness = new ModifyGraphEdgeWeightsBenchmarks { StretchAmount = SmallestStretchAmount };
        harness.Setup();

        return harness;
    }
}
