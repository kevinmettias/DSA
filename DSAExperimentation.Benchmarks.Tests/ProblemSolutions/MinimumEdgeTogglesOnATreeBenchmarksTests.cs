using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumEdgeTogglesOnATreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a brute-force DFS over an adjacency list the arm builds
// itself against a fold over a prebuilt ToggleTree - so a harness whose arms disagree is timing two
// different trees. Answers come back one per toggled edge, and the order is part of the answer: both
// strategies report the edges in the same index order, so AnswerText.Of and not OfUnorderedSet is the
// rendering that keeps each entry scored against its own edge. Setup builds the tree and applies a real
// sequence of toggles to it from one seeded stream, so the same NodeCount must rebuild the same tree.
public sealed partial class MinimumEdgeTogglesOnATreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTreeAndToggles() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceDfs()),
            AnswerText.Of(BuildHarness().BruteForceDfs()));

    [Fact]
    public void BruteForceDfs_SeededToggleTree_AgreesWithTreeFold()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.TreeFold()), AnswerText.Of(harness.BruteForceDfs()));
    }

    [Fact]
    public void TreeFold_SeededToggleTree_AgreesWithBruteForceDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForceDfs()), AnswerText.Of(harness.TreeFold()));
    }

    private static MinimumEdgeTogglesOnATreeBenchmarks BuildHarness()
    {
        var harness = new MinimumEdgeTogglesOnATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
