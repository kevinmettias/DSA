using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNodeII;

// LeetCode 117. Populating Next Right Pointers in Each Node II: same "connect same
// -depth nodes left to right" contract as #116, but the input is now an arbitrary
// binary tree instead of a guaranteed-perfect one. LevelGroupedBreadthFirstTraversal
// never assumed perfection to begin with - BinaryTreeChildren already compacts away
// null Left/Right slots (Left-then-Right order) before a level is grouped, so a
// missing sibling just means that level's buffer is shorter, with no separate
// code path needed for the general case.
public sealed partial class PopulatingNextRightPointersInEachNodeIITests
{
    [Fact]
    public void Connect_TreeWithMissingChildren_LinksAcrossGapsInTheLevel()
    {
        // [1,2,3,4,5,null,7] -> 5's next must "reach across" 3's missing left child to land on 7.
        var root = new BinaryTreeNode<int>(1)
        {
            Left = new(2) { Left = new(4), Right = new(5) },
            Right = new(3) { Right = new(7) },
        };

        var next = Connect(root);

        Assert.Null(NextOf(next, root));
        Assert.Equal(root.Right, NextOf(next, root.Left!));
        Assert.Null(NextOf(next, root.Right!));
        Assert.Equal(root.Left!.Right, NextOf(next, root.Left!.Left!));
        Assert.Equal(root.Right!.Right, NextOf(next, root.Left!.Right!));
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
