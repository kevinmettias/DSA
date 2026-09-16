using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeLevelOrderTraversalBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - one queue per level against one queue walking the
// tree and splitting on level boundaries - so a harness whose arms disagree is timing two different
// problems. AnswerText.Of, not OfUnorderedSet: level order pins the outer order (top level first)
// and the inner order (left before right within a level), so both are part of this answer. The
// class carries no [Params] - its tree is a fixed five-node literal - so the harness is built as-is.
public sealed partial class BinaryTreeLevelOrderTraversalBenchmarksTests
{
    [Fact]
    public void Setup_FiveNodeTree_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().QueueLevels()),
            AnswerText.Of(BuildHarness().QueueLevels()));

    [Fact]
    public void QueueLevels_LevelOrderedTree_AgreesWithLevelGroupedTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LevelGroupedTraversal()), AnswerText.Of(harness.QueueLevels()));
    }

    [Fact]
    public void LevelGroupedTraversal_LevelOrderedTree_AgreesWithQueueLevels()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.QueueLevels()), AnswerText.Of(harness.LevelGroupedTraversal()));
    }

    private static BinaryTreeLevelOrderTraversalBenchmarks BuildHarness()
    {
        var harness = new BinaryTreeLevelOrderTraversalBenchmarks();
        harness.Setup();

        return harness;
    }
}
