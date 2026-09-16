using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeLevelOrderTraversalIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - collect each level and reverse the level list at
// the end against level-grouped collection that inserts each level at the front - so a harness whose
// arms disagree is timing two different problems. AnswerText.Of, not OfUnorderedSet: bottom-up level
// order pins the outer order (deepest level first) as much as it pins left-before-right inside each
// level, so both are part of this answer. The class carries no [Params] - its tree is a fixed
// five-node literal - so the harness is built as-is.
public sealed partial class BinaryTreeLevelOrderTraversalIIBenchmarksTests
{
    [Fact]
    public void Setup_FiveNodeTree_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().QueueThenReverse()),
            AnswerText.Of(BuildHarness().QueueThenReverse()));

    [Fact]
    public void QueueThenReverse_BottomUpLevelOrderedTree_AgreesWithLevelGroupedThenReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LevelGroupedThenReverse()), AnswerText.Of(harness.QueueThenReverse()));
    }

    [Fact]
    public void LevelGroupedThenReverse_BottomUpLevelOrderedTree_AgreesWithQueueThenReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.QueueThenReverse()), AnswerText.Of(harness.LevelGroupedThenReverse()));
    }

    private static BinaryTreeLevelOrderTraversalIIBenchmarks BuildHarness()
    {
        var harness = new BinaryTreeLevelOrderTraversalIIBenchmarks();
        harness.Setup();

        return harness;
    }
}
