using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FlattenBinaryTreeToLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlattenBinaryTreeToLinkedListBenchmarks (ARCHITECTURE 17.9). Both arms return
// void - each flattens a clone it discards - so the harness itself exposes no value to compare, and
// the arms are reconciled the way FlattenAMultilevelDoublyLinkedList and DeleteNodeInALinkedList are:
// each [Fact] replays the arm's own exact expression on the fixture [GlobalSetup] documents, and the
// replayed chain is pinned to a preorder oracle derived independently of the fixture.
//
// That oracle is what makes the agreement meaningful. BinaryTrees.Balanced builds a complete tree in
// heap layout - node i's children at 2i + 1 and 2i + 2, node i's value i - so the preorder sequence
// is fixed by the layout arithmetic alone. Both arms are therefore asserted against the same
// independently derived chain, which is exactly the claim that they agree with each other.
public sealed partial class FlattenBinaryTreeToLinkedListBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    // Heap-layout offsets, restated from the fixture's documented child arithmetic.
    private const int LeftChildOffset = 1;
    private const int RightChildOffset = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        // The harness's own tree is private, so the workload is stated against the one expression
        // [GlobalSetup] builds it from; the arm call proves the harness is constructible.
        BuildHarness().RecursiveSplice();

        Assert.Equal(ExpectedPreorder(SmallestNodeCount), PreorderOf(BinaryTrees.Balanced(SmallestNodeCount)));
    }

    [Fact]
    public void RecursiveSplice_AgreesWithTopDownPreorderRelinkOnThePreorderChain()
    {
        BuildHarness().RecursiveSplice();

        var root = BinaryTrees.Balanced(SmallestNodeCount);

        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(root);

        Assert.Equal(ExpectedPreorder(SmallestNodeCount), RightChain(root));
    }

    [Fact]
    public void TopDownPreorderRelink_AgreesWithRecursiveSpliceOnThePreorderChain()
    {
        BuildHarness().TopDownPreorderRelink();

        var root = BinaryTrees.Balanced(SmallestNodeCount);

        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(root);

        Assert.Equal(ExpectedPreorder(SmallestNodeCount), RightChain(root));
    }

    // The preorder read straight off the heap layout: node i, then its 2i + 1 subtree, then its
    // 2i + 2 subtree - derived here rather than read back out of the fixture.
    private static int[] ExpectedPreorder(int nodeCount)
    {
        var values = new List<int>();
        AppendPreorder(values, 0, nodeCount);

        return [.. values];
    }

    private static void AppendPreorder(List<int> values, int index, int nodeCount)
    {
        if (index >= nodeCount)
        {
            return;
        }

        values.Add(index);
        AppendPreorder(values, (AlgorithmConstants.BranchingFactor * index) + LeftChildOffset, nodeCount);
        AppendPreorder(values, (AlgorithmConstants.BranchingFactor * index) + RightChildOffset, nodeCount);
    }

    private static int[] PreorderOf(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendTreePreorder(values, root);

        return [.. values];
    }

    private static void AppendTreePreorder(List<int> values, BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        AppendTreePreorder(values, node.Left);
        AppendTreePreorder(values, node.Right);
    }

    // The flattened answer: a right-only chain carrying that preorder, with every Left cleared.
    private static int[] RightChain(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();

        for (var node = root; node is not null; node = node.Right)
        {
            values.Add(node.Value);
            Assert.Null(node.Left);
        }

        return [.. values];
    }

    private static FlattenBinaryTreeToLinkedListBenchmarks BuildHarness()
    {
        var harness = new FlattenBinaryTreeToLinkedListBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
