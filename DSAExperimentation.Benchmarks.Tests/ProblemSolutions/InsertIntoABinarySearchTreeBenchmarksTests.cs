using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InsertIntoABinarySearchTree;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertIntoABinarySearchTreeBenchmarks (ARCHITECTURE 17.9). Unlike the rest
// of this tier, these two arms do NOT agree on the value they report and are not expected to:
// LeetCode 701 accepts any resulting BST that stays ordered and holds every original value plus
// the new one, and the solution's own comment says so, so the rebuilt-balanced arm's root is the
// median of the value set while the repo-BST arm's root is whichever key was inserted first. The
// root value each arm returns is a proxy for the whole tree, and the one thing the two arms do
// share is the value set both are asked to produce - so each [Fact] asserts its arm's own root
// against a value derived here from the fixture's documented shape (a shuffled stride-two
// insertion order plus the odd new key), and then asserts that the same solution call this arm
// makes over that same order builds a tree carrying exactly the expected values in order. Nothing
// is changed to force the two roots together; a harness decision to report the value set instead
// of the root belongs to the campaign owner, not here. Each arm rebuilds its own tree inside the
// measured call, so one harness is safe to call twice in either order.
public sealed partial class InsertIntoABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    // Both restated from [GlobalSetup]: the existing keys are every second value, and the new key
    // is odd so it always lands between two of them.
    private const int ValueStride = 2;
    private const int NewValue = 1;
    private const int ShuffleSeed = 1;

    // The index a sorted range is split at, the same HalvingFactor BuildBalanced divides by.
    private const int HalvingFactor = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameInsertionOrder()
    {
        Assert.Equal(BuildHarness().BinarySearchTreeInsert(), BuildHarness().BinarySearchTreeInsert());
        Assert.Equal(BuildHarness().CollectSortInsertRebuild(), BuildHarness().CollectSortInsertRebuild());
    }

    [Fact]
    public void CollectSortInsertRebuild_RebuiltBalancedTree_RootsAtTheSortedMiddle()
    {
        var harness = BuildHarness();

        Assert.Equal(SortedMiddleValue(SmallestNodeCount), harness.CollectSortInsertRebuild());
        Assert.Equal(
            ExpectedTreeValues(SmallestNodeCount),
            InOrderValues(InsertIntoABinarySearchTreeSolution.InsertByCollectSortRebuild(
                InsertionOrder(SmallestNodeCount), NewValue)));
    }

    [Fact]
    public void BinarySearchTreeInsert_PlainBstInsert_KeepsTheFirstInsertedKeyAsRoot()
    {
        var harness = BuildHarness();

        Assert.Equal(InsertionOrder(SmallestNodeCount)[0], harness.BinarySearchTreeInsert());
        Assert.Equal(
            ExpectedTreeValues(SmallestNodeCount),
            InOrderValues(InsertIntoABinarySearchTreeSolution.InsertByBstInsert(
                InsertionOrder(SmallestNodeCount), NewValue)));
    }

    private static InsertIntoABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new InsertIntoABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // [GlobalSetup]'s insertion order, restated: every second value from zero, Fisher-Yates
    // shuffled from a fixed seed. A plain BST insert leaves the first key it sees at the root.
    private static int[] InsertionOrder(int nodeCount)
    {
        var values = Enumerable.Range(0, nodeCount).Select(value => value * ValueStride).ToArray();
        var random = new Random(ShuffleSeed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }

    // The fixture's value set in the order LeetCode 701's answer must hold it: the stride-two keys
    // sorted, with the odd new key folded in at its sorted position.
    private static int[] ExpectedTreeValues(int nodeCount)
    {
        var values = new List<int> { 0, NewValue };

        for (var index = 1; index < nodeCount; index++)
        {
            values.Add(index * ValueStride);
        }

        return [.. values];
    }

    // BuildBalanced recurses on low = 0 and high = count - 1, and the value set holds
    // nodeCount + 1 entries, so the rebuilt tree's root is the value at index nodeCount / 2.
    private static int SortedMiddleValue(int nodeCount) => ExpectedTreeValues(nodeCount)[nodeCount / HalvingFactor];

    private static int[] InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendInOrder(values, root);

        return [.. values];
    }

    private static void AppendInOrder(List<int> values, BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(values, node.Left);
        values.Add(node.Value);
        AppendInOrder(values, node.Right);
    }
}
