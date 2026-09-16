using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConvertSortedListToBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): the class
// has a single arm - the original benchmark's two [Benchmark] methods were an unimplemented
// placeholder returning the literal 1 - so there is no second strategy to reconcile it against and
// the assertion has to come from what LeetCode 109's own contract makes decisive instead: the tree
// must carry the ascending list in order, and height-balanced means it stands at the minimum height
// its node count admits. Setup builds that ascending list with a constant step, so the same Length
// must rebuild the same tree, asserted through the one observable the arm exposes, its tree.
public sealed partial class ConvertSortedListToBinarySearchTreeBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the generator's own step: the list Setup builds is 0, ValueStep, 2 * ValueStep, ...
    private const int ValueStep = 3;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameTree()
    {
        Assert.Equal(InOrderValues(Built()), InOrderValues(Built()));
    }

    [Fact]
    public void MidpointRecursion_TwoHundredAscendingListValues_ReturnsTheMinimumHeightBstOverThem()
    {
        var root = Built();

        Assert.Equal(AscendingListValues(), InOrderValues(root));
        Assert.Equal(MinimumBalancedHeight(SmallestLength), Height(root));
    }

    private static BinaryTreeNode<int>? Built() =>
        (BinaryTreeNode<int>?)BuildHarness().MidpointRecursion();

    private static ConvertSortedListToBinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new ConvertSortedListToBinarySearchTreeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static List<int> InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendInOrder(root, values);

        return values;
    }

    private static void AppendInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(node.Left, values);
        values.Add(node.Value);
        AppendInOrder(node.Right, values);
    }

    private static IEnumerable<int> AscendingListValues() =>
        Enumerable.Range(0, SmallestLength).Select(index => index * ValueStep);

    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    // The shortest height any binary tree of this many nodes can have: a tree of height h holds at
    // most 2^h - 1 nodes.
    private static int MinimumBalancedHeight(int nodeCount)
    {
        var height = 0;
        var capacity = 0;

        while (capacity < nodeCount)
        {
            height++;
            capacity = (capacity * 2) + 1;
        }

        return height;
    }
}
