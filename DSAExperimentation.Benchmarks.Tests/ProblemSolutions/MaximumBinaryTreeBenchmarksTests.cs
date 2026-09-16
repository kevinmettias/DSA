using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the maximum binary tree over the array - so a harness whose arms
// disagree is timing two different problems, and both now return the built tree rather than a
// height proxy.
//
// Both arms are declared as returning object, so rendering the answer through AnswerText would
// compare two default ToString results - the type name - and pass no matter which tree either arm
// built. Each answer is therefore rendered structurally instead. Setup pins the workload to
// Enumerable.Range, an ascending run, and the maximum binary tree of an ascending run is fixed by
// that shape alone: the maximum sits last, so the root takes the largest value and every left child
// takes the next one down while every right subtree stays empty. The oracle below is derived from
// that layout, not read back out of either arm.
public sealed partial class MaximumBinaryTreeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Stands in for an absent child so a missing subtree renders differently from a present one.
    private const string EmptyNode = "#";

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(RenderTree(BuildHarness().RescanForMax()), RenderTree(BuildHarness().RescanForMax()));
        Assert.Equal(ExpectedAscendingChain(SmallestLength), RenderTree(BuildHarness().RescanForMax()));
    }

    [Fact]
    public void RescanForMax_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(RenderTree(harness.RescanForMax()), RenderTree(harness.MonotonicStack()));
    }

    [Fact]
    public void MonotonicStack_AgreesWithRescanForMax()
    {
        var harness = BuildHarness();

        Assert.Equal(RenderTree(harness.MonotonicStack()), RenderTree(harness.RescanForMax()));
    }

    private static MaximumBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumBinaryTreeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The preorder shape of the whole tree, with an explicit marker for every absent child, so two
    // structurally different trees can never render the same text.
    private static string RenderTree(object? answer) => RenderNode((BinaryTreeNode<int>?)answer);

    private static string RenderNode(BinaryTreeNode<int>? node) =>
        node is null
            ? EmptyNode
            : $"{node.Value}({RenderNode(node.Left)},{RenderNode(node.Right)})";

    // The ascending run's own maximum binary tree: value n - 1 at the root, its left child n - 2, and
    // so on down to 0, with a right subtree that is always empty.
    private static string ExpectedAscendingChain(int nodeCount) =>
        nodeCount == 0
            ? EmptyNode
            : $"{nodeCount - 1}({ExpectedAscendingChain(nodeCount - 1)},{EmptyNode})";
}
