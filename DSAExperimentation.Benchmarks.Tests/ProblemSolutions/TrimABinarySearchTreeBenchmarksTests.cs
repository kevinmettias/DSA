using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrimABinarySearchTreeBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins what the class comment claims about the workload, from the
// workload's construction alone.
//
// low/high span the tree's whole value range, so no node is out of range, and LC 669 forbids a trim
// from changing the relative structure of the nodes it keeps: the answer must be Setup's own tree,
// unchanged, and calling an arm must leave the tree as it found it - which is what lets every
// invocation trim the same tree.
public sealed partial class TrimABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const int ShuffleSeed = 669;

    [Fact]
    public void InPlaceTrim_WholeValueRange_ReturnsSetupsTreeUnchanged() =>
        Assert.Equal(SetupTree(), AnswerGraphText.Of(BuildHarness().InPlaceTrim()));

    [Fact]
    public void IterativeBoundaryWalk_WholeValueRange_ReturnsSetupsTreeUnchanged() =>
        Assert.Equal(SetupTree(), AnswerGraphText.Of(BuildHarness().IterativeBoundaryWalk()));

    // The shuffled insertion is what keeps the tree shallow; a sorted insertion would build a chain
    // and make the boundary walk visit every node, which is the comparison the class comment rules out.
    [Fact]
    public void Setup_ShuffledInsertion_BuildsATreeFarShallowerThanAChain()
    {
        var height = HeightOf((BinaryTreeNode<int>?)BuildHarness().InPlaceTrim());

        Assert.InRange(height, 1, SmallestNodeCount / 10);
    }

    // [GlobalSetup]'s tree, restated: 1..NodeCount inserted in the same seeded shuffled order.
    private static string SetupTree()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in SeededSequences.ShuffledOneTo(SmallestNodeCount, ShuffleSeed))
        {
            tree.Insert(value);
        }

        return AnswerGraphText.Of(tree.Root);
    }

    private static int HeightOf(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));

    private static TrimABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new TrimABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
