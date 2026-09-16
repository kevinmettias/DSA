using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarks
// (ARCHITECTURE 17.9): both arms answer the same question - the "U"/"L"/"R" directions LC 2096
// asks for between two nodes of one tree - one by searching for the path directly, one via the
// lowest common ancestor, so a harness whose arms disagree is timing two different problems.
// Setup rebuilds a balanced tree from a seeded node count, so the same count must rebuild the
// same endpoints.
public sealed partial class StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DirectPathSearch()),
            AnswerText.Of(BuildHarness().DirectPathSearch()));

    [Fact]
    public void DirectPathSearch_AgreesWithLowestCommonAncestorWithRootToLeafPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.DirectPathSearch(),
            harness.LowestCommonAncestorWithRootToLeafPaths());
    }

    [Fact]
    public void LowestCommonAncestorWithRootToLeafPaths_AgreesWithDirectPathSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.LowestCommonAncestorWithRootToLeafPaths(),
            harness.DirectPathSearch());
    }

    private static StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarks BuildHarness()
    {
        var harness = new StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarks
        {
            NodeCount = SmallestNodeCount,
        };

        harness.Setup();

        return harness;
    }
}
