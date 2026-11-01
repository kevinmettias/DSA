using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumBinaryTreeBenchmarks (ARCHITECTURE 17.9): the arms return the built
// tree, and Setup pins the workload to Enumerable.Range, an ascending run, whose maximum binary tree
// is fixed by that shape alone: the maximum sits last, so the root takes the largest value and every
// left child takes the next one down while every right subtree stays empty. The oracle below is
// derived from that layout, not read back out of either arm - an expected value the generic
// agreement check, which only compares arms with each other, cannot supply.
public sealed partial class MaximumBinaryTreeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Stands in for an absent child so a missing subtree renders differently from a present one.
    private const string EmptyNode = "#";

    [Fact]
    public void RescanForMax_AscendingRun_BuildsALeftLeaningChain() =>
        Assert.Equal(ExpectedAscendingChain(SmallestLength), RenderTree(BuildHarness().RescanForMax()));

    private static MaximumBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumBinaryTreeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The preorder shape of the whole tree, with an explicit marker for every absent child, so two
    // structurally different trees can never render the same text.
    private static string RenderTree(BinaryTreeNode<int>? node) =>
        node is null
            ? EmptyNode
            : $"{node.Value}({RenderTree(node.Left)},{RenderTree(node.Right)})";

    // The ascending run's own maximum binary tree: value n - 1 at the root, its left child n - 2, and
    // so on down to 0, with a right subtree that is always empty.
    private static string ExpectedAscendingChain(int nodeCount) =>
        nodeCount == 0
            ? EmptyNode
            : $"{nodeCount - 1}({ExpectedAscendingChain(nodeCount - 1)},{EmptyNode})";
}
