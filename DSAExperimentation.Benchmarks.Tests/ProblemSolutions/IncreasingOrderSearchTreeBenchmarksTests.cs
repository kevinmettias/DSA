using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.IncreasingOrderSearchTree;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IncreasingOrderSearchTreeBenchmarks (ARCHITECTURE 17.9). Both arms are
// competing strategies for the same question - a hand-rolled recursive in-order walk threading a
// tail through parameters against this repo's InOrderTraversal/IInOrderHooks doing the identical
// relink - so a harness whose arms disagree is timing two different problems. Each arm returns
// the relinked chain's root value, an int, which is a proxy for the whole rearrangement: it pins
// the one node the problem's own answer names (LC 897 makes the smallest value the root) but says
// nothing about the rest of the chain. The agreement is therefore honest and weak on its own, so
// the InOrderTraversalHooks [Fact] also replays the arm's own expression on the tree [GlobalSetup]
// documents and checks every value in the resulting chain, derived independently from the
// fixture. That chain is decisive: [GlobalSetup] shuffles the values 1..NodeCount with a fixed
// seed, so an in-order relink must produce exactly 1..NodeCount ascending with no left child,
// and the root must be the lowest shuffled value. Both arms rebuild a fresh tree inside the call
// - the walks relink Left/Right in place - so one harness is safe to call twice in either order.
public sealed partial class IncreasingOrderSearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    // SeededSequences.ShuffledOneTo starts at one, so the ascending chain starts here too.
    private const int LowestShuffledValue = 1;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameShuffledValues()
    {
        Assert.Equal(BuildHarness().RecursiveRelink(), BuildHarness().RecursiveRelink());
        Assert.Equal(BuildHarness().InOrderTraversalHooks(), BuildHarness().InOrderTraversalHooks());
    }

    [Fact]
    public void RecursiveRelink_ShuffledValues_AgreesWithInOrderTraversalHooks()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InOrderTraversalHooks(), harness.RecursiveRelink());
        Assert.Equal(LowestShuffledValue, harness.RecursiveRelink());
    }

    [Fact]
    public void InOrderTraversalHooks_ShuffledValues_AgreesWithRecursiveRelink()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveRelink(), harness.InOrderTraversalHooks());
        Assert.Equal(
            AscendingValues(SmallestNodeCount),
            ChainValues(IncreasingOrderSearchTreeSolution.IncreasingBstByInOrderHooks(BuildTree(SmallestNodeCount))));
    }

    private static IncreasingOrderSearchTreeBenchmarks BuildHarness()
    {
        var harness = new IncreasingOrderSearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static BinaryTreeNode<int> BuildTree(int nodeCount)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in SeededSequences.ShuffledOneTo(nodeCount, seed: 1))
        {
            tree.Insert(value);
        }

        return tree.Root!;
    }

    // LC 897's answer read straight off the fixture's documented values, not out of the walk.
    private static int[] AscendingValues(int nodeCount) => [.. Enumerable.Range(LowestShuffledValue, nodeCount)];

    private static int[] ChainValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();

        for (var node = root; node is not null; node = node.Right)
        {
            values.Add(node.Value);
            Assert.Null(node.Left);
        }

        return [.. values];
    }
}
