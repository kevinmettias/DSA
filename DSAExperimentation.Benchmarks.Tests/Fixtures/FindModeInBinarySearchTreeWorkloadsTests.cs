using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FindModeInBinarySearchTreeWorkloads (ARCHITECTURE 17.7). The reading depends
// on LC 501's tree carrying realistic duplicate runs - every value repeating about
// nodesPerDistinctValue times - and on it being a genuine search tree with duplicates allowed to
// the left, which is the shape both strategies' in-order walks read.
public sealed partial class FindModeInBinarySearchTreeWorkloadsTests
{
    private const int NodeCount = 64;
    private const int NodesPerDistinctValue = 4;
    private const int Seed = 501; // LC problem number

    [Fact]
    public void BuildTree_NodeCount_ReturnsExactlyThatManyNodes() =>
        Assert.Equal(
            NodeCount,
            CountNodes(FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, Seed)));

    [Fact]
    public void BuildTree_EveryValue_RepeatsNodesPerDistinctValueTimes()
    {
        var root = FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, Seed);
        var counts = Nodes(root)
            .GroupBy(node => node.Value)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(NodeCount / NodesPerDistinctValue, counts.Count);
        Assert.All(counts.Values, count => Assert.Equal(NodesPerDistinctValue, count));
    }

    // LC 501 explicitly permits repeated values, and the fixture's own note says a duplicate goes
    // left (value <= node.Value), which this repo's BinarySearchTree<TValue>.Insert would instead
    // reject. That left-side convention is what the mode walk reads.
    [Fact]
    public void BuildTree_EveryChild_RespectsTheDuplicateGoesLeftSearchOrder()
    {
        var root = FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, Seed);

        Assert.All(Nodes(root), node =>
        {
            if (node.Left is { } left)
            {
                Assert.True(left.Value <= node.Value);
            }

            if (node.Right is { } right)
            {
                Assert.True(right.Value > node.Value);
            }
        });
    }

    [Fact]
    public void BuildTree_SameSeed_ReturnsTheSameTree() =>
        Assert.Equal(
            InOrderValues(FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, Seed)),
            InOrderValues(FindModeInBinarySearchTreeWorkloads.BuildTree(NodeCount, NodesPerDistinctValue, Seed)));

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static List<BinaryTreeNode<int>> Nodes(BinaryTreeNode<int>? root)
    {
        var nodes = new List<BinaryTreeNode<int>>();
        var pending = new Queue<BinaryTreeNode<int>>();

        if (root is not null)
        {
            pending.Enqueue(root);
        }

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            nodes.Add(node);

            if (node.Left is { } left)
            {
                pending.Enqueue(left);
            }

            if (node.Right is { } right)
            {
                pending.Enqueue(right);
            }
        }

        return nodes;
    }

    private static List<int> InOrderValues(BinaryTreeNode<int>? node)
    {
        var values = new List<int>();

        if (node is null)
        {
            return values;
        }

        values.AddRange(InOrderValues(node.Left));
        values.Add(node.Value);
        values.AddRange(InOrderValues(node.Right));

        return values;
    }
}
