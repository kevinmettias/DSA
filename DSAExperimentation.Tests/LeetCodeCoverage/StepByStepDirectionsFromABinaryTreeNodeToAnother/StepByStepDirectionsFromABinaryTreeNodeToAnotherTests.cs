using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StepByStepDirectionsFromABinaryTreeNodeToAnother;

// LeetCode 2096. Step-By-Step Directions From a Binary Tree Node to Another: reuses
// this repo's own LowestCommonAncestor (the same engine LowestCommonAncestorOfBstTests
// composes) to find where the start->dest route turns around, then AllRootToLeafPaths
// (already proven by PathSumII/PathSumIII/KthAncestorOfATreeNode) rooted AT that LCA
// to recover the two root-to-node chains needed to build the "U"/"L"/"R" string - one
// "U" per edge climbing from start up to the LCA, then one "L" or "R" per edge
// descending from the LCA down to dest depending on which child pointer each step
// follows.
public sealed partial class StepByStepDirectionsFromABinaryTreeNodeToAnotherTests
{
    [Fact]
    public void GetDirections_LeetCodeExampleOne_ReturnsUpThenLeftThenRight()
    {
        // root: 5
        //      / \
        //     1   2
        //    /   / \
        //   3   6   4
        var three = new BinaryTreeNode<int>(3);
        var six = new BinaryTreeNode<int>(6);
        var four = new BinaryTreeNode<int>(4);
        var one = new BinaryTreeNode<int>(1) { Left = three };
        var two = new BinaryTreeNode<int>(2) { Left = six, Right = four };
        var root = new BinaryTreeNode<int>(5) { Left = one, Right = two };

        var directions = GetDirections(root, three, six);

        Assert.Equal("UURL", directions);
    }

    [Fact]
    public void GetDirections_DestinationIsDescendantOfStart_ReturnsOnlyDownwardMoves()
    {
        var grandchild = new BinaryTreeNode<int>(3);
        var child = new BinaryTreeNode<int>(2) { Right = grandchild };
        var root = new BinaryTreeNode<int>(1) { Left = child };

        var directions = GetDirections(root, root, grandchild);

        Assert.Equal("LR", directions);
    }

    [Fact]
    public void GetDirections_StartIsAncestorOfDestination_ReturnsOnlyUpwardMoves()
    {
        var grandchild = new BinaryTreeNode<int>(3);
        var child = new BinaryTreeNode<int>(2) { Right = grandchild };
        var root = new BinaryTreeNode<int>(1) { Left = child };

        var directions = GetDirections(root, grandchild, root);

        Assert.Equal("UU", directions);
    }

    private static string GetDirections(BinaryTreeNode<int> root, BinaryTreeNode<int> start, BinaryTreeNode<int> dest)
    {
        var lca = FindLowestCommonAncestor(root, start, dest);
        var leafPaths = FindLeafPaths(lca);

        var pathToStart = PathTo(leafPaths, start);
        var pathToDest = PathTo(leafPaths, dest);

        var up = BuildUpwardMoves(pathToStart);
        var down = BuildDownwardMoves(pathToDest);

        return up + down;
    }

    private static BinaryTreeNode<int> FindLowestCommonAncestor(
        BinaryTreeNode<int> root, BinaryTreeNode<int> start, BinaryTreeNode<int> dest)
    {
        var lca = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root, start, dest);

        // presumption: allow -- start and dest are always nodes reachable from root in
        // the trees these tests build, so Find can only return null when neither is
        // (impossible here), the same guarantee LowestCommonAncestorOfBstTests relies on.
        return lca!;
    }

    private static List<BinaryTreeNode<int>[]> FindLeafPaths(BinaryTreeNode<int> lca)
        => AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(lca);

    private static string BuildUpwardMoves(BinaryTreeNode<int>[] pathToStart) => new('U', pathToStart.Length - 1);

    private static string BuildDownwardMoves(BinaryTreeNode<int>[] pathToDest)
    {
        var down = new char[pathToDest.Length - 1];

        for (var i = 0; i < down.Length; i++)
        {
            down[i] = pathToDest[i].Left == pathToDest[i + 1] ? 'L' : 'R';
        }

        return new string(down);
    }

    // Every leaf path AllRootToLeafPaths returns starts at the same root (here, the
    // LCA); target's own root-to-node chain is whichever leaf path contains it,
    // truncated right after target.
    private static BinaryTreeNode<int>[] PathTo(List<BinaryTreeNode<int>[]> leafPaths, BinaryTreeNode<int> target)
    {
        foreach (var path in leafPaths)
        {
            var index = Array.IndexOf(path, target);

            if (index >= 0)
            {
                return path[..(index + 1)];
            }
        }

        throw new InvalidOperationException("target is not reachable from the given root.");
    }
}
