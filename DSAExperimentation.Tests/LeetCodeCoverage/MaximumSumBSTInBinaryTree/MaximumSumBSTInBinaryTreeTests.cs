using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumBSTInBinaryTree;

// LeetCode 1373. Maximum Sum BST in Binary Tree: one bottom-up post-order pass over
// this repo's own BinaryTreeNode<int>, combining each node's two subtree summaries
// (IsBst, Min, Max, Sum) - the same direct-recursion composition ValidateBinaryS
// earchTreeTests already uses to check BST-ness bottom-up and BinaryTreeMaximumPath
// SumTests already uses to thread a running "best" through the walk. TreeFold isn't
// a fit here: BinaryTreeChildren compacts away a missing Left/Right (its own doc
// comment explains why), and this algorithm's every combine step needs to know
// specifically whether left.Max or right.Min came from an *actual* left/right child
// versus an absent one - exactly the positional identity that compaction throws away.
public sealed partial class MaximumSumBSTInBinaryTreeTests
{
    [Fact]
    public void MaxSumBST_RootBreaksBstButALeftSubtreeIsValid_ReturnsThatSubtreeSum()
    {
        // Root 5 with left child 8 already breaks BST order (8 > 5), so the whole
        // tree is invalid - but the subtree rooted at 8 (8, 3, 10) is itself a
        // valid BST and beats every smaller valid subtree's sum (10, 3, -2).
        var root = new BinaryTreeNode<int>(5)
        {
            Left = new BinaryTreeNode<int>(8)
            {
                Left = new BinaryTreeNode<int>(3),
                Right = new BinaryTreeNode<int>(10),
            },
            Right = new BinaryTreeNode<int>(-2),
        };

        Assert.Equal(21, MaxSumBST(root));
    }

    [Fact]
    public void MaxSumBST_EntireTreeIsAlreadyAValidBst_ReturnsTotalSum()
    {
        var root = new BinaryTreeNode<int>(2) { Left = new(1), Right = new(3) };

        Assert.Equal(6, MaxSumBST(root));
    }

    [Fact]
    public void MaxSumBST_SingleNode_ReturnsItsOwnValue()
    {
        var root = new BinaryTreeNode<int>(7);

        Assert.Equal(7, MaxSumBST(root));
    }

    private static int MaxSumBST(BinaryTreeNode<int>? root)
    {
        // Starts at int.MinValue, not 0: a single node is always a trivially valid
        // BST, so the true answer can itself be negative when every value in the
        // tree is negative - there is no implicit "or zero" floor in the problem.
        var best = int.MinValue;
        Scan(root);
        return best;

        Summary Scan(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return new Summary(IsBst: true, Min: int.MaxValue, Max: int.MinValue, Sum: 0);
            }

            var left = Scan(node.Left);
            var right = Scan(node.Right);
            var isBst = left.IsBst && right.IsBst && node.Value > left.Max && node.Value < right.Min;

            if (!isBst)
            {
                return new Summary(false, 0, 0, 0);
            }

            var sum = left.Sum + right.Sum + node.Value;
            best = Math.Max(best, sum);

            return new Summary(true, Math.Min(node.Value, left.Min), Math.Max(node.Value, right.Max), sum);
        }
    }

    private readonly record struct Summary(bool IsBst, int Min, int Max, int Sum);
}
