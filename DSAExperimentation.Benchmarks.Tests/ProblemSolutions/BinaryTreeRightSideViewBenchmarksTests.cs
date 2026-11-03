using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinaryTreeRightSideViewBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to each other, and this pins the baseline's view to a value derived without any arm.
// Fixtures.BinaryTrees.Balanced makes the answer decisive: node values are their heap indices, so
// level d holds indices [2^d - 1, 2^(d+1) - 2] and the right side view is the last index on each
// level, clipped to the node count when the bottom level is partial. AnswerGraphText.Of, not
// OfUnordered: one value per level with the levels in order is what a right side view is.
public sealed partial class BinaryTreeRightSideViewBenchmarksTests
{
    private const int SmallestNodeCount = 15;

    [Fact]
    public void DepthFirstRightFirst_CompleteTreeWithFullLevels_ReturnsTheRightmostIndexPerLevel() =>
        Assert.Equal(
            AnswerGraphText.Of(ExpectedRightSideView(SmallestNodeCount)),
            AnswerGraphText.Of(BuildHarness().DepthFirstRightFirst()));

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
