using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeRightSideViewBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - the buffered-level BFS and the right-first DFS - so a
// harness whose arms disagree is reading two different trees: both must return the same view.
// Fixtures.BinaryTrees.Balanced makes the answer decisive: node values are their heap indices, so
// level d holds indices [2^d - 1, 2^(d+1) - 2] and the right side view is the last index on each
// level, clipped to the node count when the bottom level is partial. AnswerText.Of, not
// OfUnorderedSet: one value per level with the levels in order is what a right side view is.
public sealed partial class BinaryTreeRightSideViewBenchmarksTests
{
    private const int SmallestNodeCount = 255;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LevelGroupedTraversal()),
            AnswerText.Of(BuildHarness().LevelGroupedTraversal()));

    [Fact]
    public void LevelGroupedTraversal_CompleteTreeWithFullLevels_ReturnsTheRightmostIndexPerLevel() =>
        Assert.Equal(
            AnswerText.Of(ExpectedRightSideView(SmallestNodeCount)),
            AnswerText.Of(BuildHarness().LevelGroupedTraversal()));

    [Fact]
    public void DepthFirstRightFirst_CompleteTreeWithFullLevels_ReturnsTheRightmostIndexPerLevel() =>
        Assert.Equal(
            AnswerText.Of(ExpectedRightSideView(SmallestNodeCount)),
            AnswerText.Of(BuildHarness().DepthFirstRightFirst()));

    [Fact]
    public void DepthFirstRightFirst_AgreesWithLevelGroupedTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LevelGroupedTraversal()),
            AnswerText.Of(harness.DepthFirstRightFirst()));
    }

    private static BinaryTreeRightSideViewBenchmarks BuildHarness()
    {
        var harness = new BinaryTreeRightSideViewBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // Level d starts at index 2^d - 1, so the next level starts one past the branching factor times
    // the current start; that next start is one past this level's last index, which a partial bottom
    // level clips to the last node.
    private static List<int> ExpectedRightSideView(int nodeCount)
    {
        var view = new List<int>();
        var levelStart = 0;

        while (levelStart < nodeCount)
        {
            var nextLevelStart = NextLevelStart(levelStart);
            var lastIndexOfLevel = Math.Min(nextLevelStart - 1, nodeCount - 1);
            view.Add(lastIndexOfLevel);
            levelStart = nextLevelStart;
        }

        return view;
    }

    // The same heap layout the fixture builds: node i's first child is at BranchingFactor * i + 1,
    // and the first child of a level's first node is the next level's first node.
    private static int NextLevelStart(int levelStart) =>
        (AlgorithmConstants.BranchingFactor * levelStart) + 1;
}
