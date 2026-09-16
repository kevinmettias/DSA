using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LowestCommonAncestorOfABinaryTreeBenchmarks (ARCHITECTURE 17.9). There is one
// arm, so there is no second strategy to agree with: the expected answer is derived from the fixture
// instead. BinaryTrees.Balanced fills node i with the value i and puts node i's children at 2i+1 and
// 2i+2 - a complete tree in heap layout - and the arm queries its leftmost leaf and its rightmost
// leaf. Both leaves can be found by index arithmetic alone, and so can their lowest common ancestor,
// so the oracle below never walks a tree. The node type is internal, hence the object return and the
// cast here; the two harness builds share a NodeCount, so the same NodeCount must rebuild the same
// tree and answer at the same node.
public sealed partial class LowestCommonAncestorOfABinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(ValueOf(BuildHarness().AncestryWalk()), ValueOf(BuildHarness().AncestryWalk()));

    [Fact]
    public void AncestryWalk_LeftmostAndRightmostLeaves_ReturnsTheirLowestCommonAncestor() =>
        Assert.Equal(ExpectedLcaValue(SmallestNodeCount), ValueOf(BuildHarness().AncestryWalk()));

    // A node of the heap-layout fixture carries its own index as its value, so the index the layout
    // arithmetic below derives is the value the returned node must carry.
    private static int ValueOf(object? node) => ((BinaryTreeNode<int>)node!).Value;

    private static int ExpectedLcaValue(int nodeCount) =>
        LayoutLca(LeftmostLeafIndex(nodeCount), RightmostLeafIndex(nodeCount));

    // Leftmost: descend to the left child for as long as one exists, which is exactly the child
    // preference the arm's own descent uses (a right child only exists where a left one does).
    private static int LeftmostLeafIndex(int nodeCount)
    {
        var index = 0;
        var left = (AlgorithmConstants.BranchingFactor * index) + 1;

        while (left < nodeCount)
        {
            index = left;
            left = (AlgorithmConstants.BranchingFactor * index) + 1;
        }

        return index;
    }

    // Rightmost: descend to the right child where one exists, and to the left child otherwise -
    // again the arm's preference, in the layout's own index terms.
    private static int RightmostLeafIndex(int nodeCount)
    {
        var index = 0;
        var left = (AlgorithmConstants.BranchingFactor * index) + 1;

        while (left < nodeCount)
        {
            var right = left + 1;
            index = right < nodeCount ? right : left;
            left = (AlgorithmConstants.BranchingFactor * index) + 1;
        }

        return index;
    }

    // Two heap-layout nodes share a lowest common ancestor exactly where their ancestor chains
    // (each step to (index - 1) / BranchingFactor) first meet.
    private static int LayoutLca(int first, int second)
    {
        while (first != second)
        {
            if (first > second)
            {
                first = (first - 1) / AlgorithmConstants.BranchingFactor;
            }
            else
            {
                second = (second - 1) / AlgorithmConstants.BranchingFactor;
            }
        }

        return first;
    }

    private static LowestCommonAncestorOfABinaryTreeBenchmarks BuildHarness()
    {
        var harness = new LowestCommonAncestorOfABinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
