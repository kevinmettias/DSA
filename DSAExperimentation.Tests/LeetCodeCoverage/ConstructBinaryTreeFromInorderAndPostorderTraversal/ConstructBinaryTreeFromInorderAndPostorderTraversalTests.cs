using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromInorderAndPostorderTraversal;

public sealed partial class ConstructBinaryTreeFromInorderAndPostorderTraversalTests
{
    [Fact]
    public void BuildTree_ClassicExample_ReconstructsBinaryTree()
    {
        var root = Build([9, 3, 15, 20, 7], [9, 15, 7, 20, 3]);
        Assert.Equal([3, 9, 20, 15, 7], PreOrder(root));
    }

    private static BinaryTreeNode<int>? Build(int[] inorder, int[] postorder)
    {
        var walk = new PostorderWalk(postorder, InorderIndex(inorder));
        var post = postorder.Length - 1;

        return BuildRange(0, inorder.Length - 1, walk, ref post);
    }

    // The postorder cursor is the recursion's own state, not Build's, so it arrives as a
    // parameter and moves down the postorder array as each root is consumed.
    private static BinaryTreeNode<int>? BuildRange(int low, int high, PostorderWalk walk, ref int post)
    {
        if (low > high)
        {
            return null;
        }

        var value = walk.Postorder[post--];
        walk.InorderIndex.TryGetValue(value, out var mid);
        return new BinaryTreeNode<int>(value)
        {
            Right = BuildRange(mid + 1, high, walk, ref post),
            Left = BuildRange(low, mid - 1, walk, ref post),
        };
    }

    // Where each value sits in the inorder walk, which is what turns a postorder root into
    // the boundary between its left and right subtrees.
    private static HashMap<int, int> InorderIndex(int[] inorder)
    {
        var positions = new HashMap<int, int>();
        for (var i = 0; i < inorder.Length; i++)
        {
            positions.Set(inorder[i], i);
        }

        return positions;
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        return [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
    }

    // The postorder values the recursion consumes and the inorder index that splits each of
    // them into two subtrees. Both are read at every step, so they travel as the one thing
    // the recursion is walking.
    private readonly record struct PostorderWalk(int[] Postorder, HashMap<int, int> InorderIndex);
}
