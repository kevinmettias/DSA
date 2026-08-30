using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubtreeOfAnotherTree;

// LeetCode 572. Subtree of Another Tree: for every node of the main tree, check
// whether the subtree rooted there is structurally identical to subRoot - the
// same node-by-node equality SameTreeTests already proves - stopping at the
// first match.
public sealed partial class SubtreeOfAnotherTreeTests
{
    [Fact]
    public void IsSubtree_MatchingSubtreeExists_ReturnsTrue()
        => Assert.True(IsSubtree(
            new BinaryTreeNode<int>(3) { Left = new(4) { Left = new(1), Right = new(2) }, Right = new(5) },
            new BinaryTreeNode<int>(4) { Left = new(1), Right = new(2) }));

    [Fact]
    public void IsSubtree_SameShapeDifferentValues_ReturnsFalse()
        => Assert.False(IsSubtree(
            new BinaryTreeNode<int>(3) { Left = new(4) { Left = new(1), Right = new(2) { Left = new(0) } }, Right = new(5) },
            new BinaryTreeNode<int>(4) { Left = new(1), Right = new(2) }));

    private static bool IsSubtree(BinaryTreeNode<int>? root, BinaryTreeNode<int> subRoot)
        => root is not null && (IsSame(root, subRoot) || IsSubtree(root.Left, subRoot) || IsSubtree(root.Right, subRoot));

    private static bool IsSame(BinaryTreeNode<int>? p, BinaryTreeNode<int>? q)
        => p is null || q is null ? p is null && q is null : p.Value == q.Value && IsSame(p.Left, q.Left) && IsSame(p.Right, q.Right);
}
