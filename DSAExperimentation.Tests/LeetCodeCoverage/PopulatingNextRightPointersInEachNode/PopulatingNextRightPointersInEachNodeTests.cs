using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNode;

// LeetCode 116. Populating Next Right Pointers in Each Node: connect each node to
// its next right neighbor at the same depth (null for the rightmost node of each
// level). BinaryTreeNode<TValue> has no Next field of its own, so the "populate"
// step is represented as a HashMap<node, nextNode> built from
// LevelGroupedBreadthFirstTraversal's own left-to-right level buffers - exactly the
// per-level grouping this problem asks for, already produced by an existing
// primitive with no perfect-tree-specific code of its own.
public sealed partial class PopulatingNextRightPointersInEachNodeTests
{
    [Fact]
    public void Connect_PerfectBinaryTree_LinksEachNodeToItsRightNeighbor()
    {
        var root = new BinaryTreeNode<int>(1)
        {
            Left = new(2) { Left = new(4), Right = new(5) },
            Right = new(3) { Left = new(6), Right = new(7) },
        };

        var next = Connect(root);

        Assert.Null(NextOf(next, root));
        Assert.Equal(root.Right, NextOf(next, root.Left!));
        Assert.Null(NextOf(next, root.Right!));
        Assert.Equal(root.Left!.Right, NextOf(next, root.Left!.Left!));
        Assert.Equal(root.Right!.Left, NextOf(next, root.Left!.Right!));
        Assert.Equal(root.Right!.Right, NextOf(next, root.Right!.Left!));
        Assert.Null(NextOf(next, root.Right!.Right!));
    }

    private static BinaryTreeNode<int>? NextOf(HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> next, BinaryTreeNode<int> node)
        => next.TryGetValue(node, out var nextNode) ? nextNode : throw new KeyNotFoundException();

    private static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> Connect(BinaryTreeNode<int> root)
    {
        LevelHooks.Output.Value = [];

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);

        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        foreach (var level in LevelHooks.Output.Value!)
        {
            for (var i = 0; i < level.Count; i++)
            {
                next.Set(level[i], i + 1 < level.Count ? level[i + 1] : null);
            }
        }

        return next;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.ToList());
    }
}
