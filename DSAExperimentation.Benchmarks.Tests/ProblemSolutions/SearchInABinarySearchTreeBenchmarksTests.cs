using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): both arms search the
// same tree for the same key, so a harness whose arms disagree is timing two different problems.
// Setup builds the tree from one shuffled insertion order seeded at 1 and aims at the highest
// value, so the same NodeCount must rebuild the same tree - and since the tree holds every value
// 1..NodeCount, that highest value is present and the node holding it is the answer both arms
// must return. Neither arm changes the tree, so one harness instance is safe to read twice in
// either order.
public sealed partial class SearchInABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;
    private const int HighestValue = SmallestNodeCount;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(AnswerGraphText.Of(BuildHarness().LinearScan()), AnswerGraphText.Of(BuildHarness().LinearScan()));
        Assert.Equal(HighestValue, Assert.IsType<BinaryTreeNode<int>>(BuildHarness().LinearScan()).Value);
    }

    [Fact]
    public void BinarySearchTreeDescent_SeededTree_FindsTheHighestValue() =>
        Assert.Equal(HighestValue, Assert.IsType<BinaryTreeNode<int>>(BuildHarness().BinarySearchTreeDescent()).Value);

    private static SearchInABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new SearchInABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
