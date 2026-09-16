using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InvertBinaryTree;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InvertBinaryTreeBenchmarks (ARCHITECTURE 17.9): the class has a single arm
// and that arm returns void - it inverts a clone it then discards - so the harness exposes no value
// to compare, and the arm is reconciled the way FlattenBinaryTreeToLinkedList is: the [Fact]
// replays the arm's own expression on the fixture [GlobalSetup] documents, and the replayed tree is
// checked against an oracle derived independently of the fixture. BinaryTrees.Balanced builds a
// complete tree in heap layout - node i's children at 2i + 1 and 2i + 2, node i's value i - so a
// mirror image traversed pre-order with the children swapped yields a fixed sequence computed from
// the layout arithmetic alone. The harness's own tree is private and Setup returns void, so the
// Setup [Fact] states the workload against the one expression [GlobalSetup] builds it from and
// calls the arm to prove the harness is constructible.
public sealed partial class InvertBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 255;

    // Heap-layout offsets, restated from the fixture's documented child arithmetic.
    private const int LeftChildOffset = 1;
    private const int RightChildOffset = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        BuildHarness().RecursiveSwap();

        Assert.Equal(
            PreorderOf(BinaryTrees.Balanced(SmallestNodeCount)),
            PreorderOf(BinaryTrees.Balanced(SmallestNodeCount)));
    }

    [Fact]
    public void RecursiveSwap_BalancedTree_MirrorsTheHeapLayout()
    {
        BuildHarness().RecursiveSwap();

        var inverted = InvertBinaryTreeSolution.InvertByRecursiveSwap(BinaryTrees.Balanced(SmallestNodeCount));

        Assert.Equal(ExpectedMirrorPreorder(SmallestNodeCount), PreorderOf(inverted));
    }

    private static InvertBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new InvertBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // The mirrored pre-order read straight off the heap layout: node i, then its 2i + 2 subtree,
    // then its 2i + 1 subtree - the children swapped, derived here rather than read back out of the
    // arm.
    private static int[] ExpectedMirrorPreorder(int nodeCount)
    {
        var values = new List<int>();
        AppendMirrorPreorder(values, 0, nodeCount);

        return [.. values];
    }

    private static void AppendMirrorPreorder(List<int> values, int index, int nodeCount)
    {
        if (index >= nodeCount)
        {
            return;
        }

        values.Add(index);
        AppendMirrorPreorder(values, (AlgorithmConstants.BranchingFactor * index) + RightChildOffset, nodeCount);
        AppendMirrorPreorder(values, (AlgorithmConstants.BranchingFactor * index) + LeftChildOffset, nodeCount);
    }

    private static int[] PreorderOf(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendPreorder(values, root);

        return [.. values];
    }

    private static void AppendPreorder(List<int> values, BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        AppendPreorder(values, node.Left);
        AppendPreorder(values, node.Right);
    }
}
