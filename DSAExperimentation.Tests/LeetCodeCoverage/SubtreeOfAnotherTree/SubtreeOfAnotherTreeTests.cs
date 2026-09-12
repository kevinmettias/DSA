using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SubtreeOfAnotherTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubtreeOfAnotherTree;

// Harness only. Both structural-match strategies are
// SubtreeOfAnotherTreeSolution's - this file just pins them to LeetCode's
// published examples plus a couple of edge cases (a whole-tree match, and a
// subRoot value that never appears in root at all). BinaryTreeNode<int> is
// internal, so - as in SameTreeTests/ValidateBinarySearchTreeTests - it stays
// out of a public TheoryData/[Theory] signature and is only ever handed to the
// solution through private helpers.
public sealed class SubtreeOfAnotherTreeTests
{
    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_MatchingSubtreeExists_ReturnsTrue() =>
        Assert.True(SubtreeOfAnotherTreeSolution.IsSubtreeByRecursiveCompareAtEveryNode(Root(), SubRoot()));

    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_SameShapeDifferentValues_ReturnsFalse() =>
        Assert.False(SubtreeOfAnotherTreeSolution.IsSubtreeByRecursiveCompareAtEveryNode(RootWithExtraLeaf(), SubRoot()));

    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_WholeTreeMatch_ReturnsTrue() =>
        Assert.True(SubtreeOfAnotherTreeSolution.IsSubtreeByRecursiveCompareAtEveryNode(Tree(5), Tree(5)));

    [Fact]
    public void IsSubtreeByRecursiveCompareAtEveryNode_SubRootValueNeverAppearsInRoot_ReturnsFalse() =>
        Assert.False(SubtreeOfAnotherTreeSolution.IsSubtreeByRecursiveCompareAtEveryNode(
            Tree(1, left: Tree(2)), Tree(3)));

    [Fact]
    public void IsSubtreeBySerializeThenKmpSearch_MatchingSubtreeExists_ReturnsTrue() =>
        Assert.True(SubtreeOfAnotherTreeSolution.IsSubtreeBySerializeThenKmpSearch(Root(), SubRoot()));

    [Fact]
    public void IsSubtreeBySerializeThenKmpSearch_SameShapeDifferentValues_ReturnsFalse() =>
        Assert.False(SubtreeOfAnotherTreeSolution.IsSubtreeBySerializeThenKmpSearch(RootWithExtraLeaf(), SubRoot()));

    [Fact]
    public void IsSubtreeBySerializeThenKmpSearch_WholeTreeMatch_ReturnsTrue() =>
        Assert.True(SubtreeOfAnotherTreeSolution.IsSubtreeBySerializeThenKmpSearch(Tree(5), Tree(5)));

    [Fact]
    public void IsSubtreeBySerializeThenKmpSearch_SubRootValueNeverAppearsInRoot_ReturnsFalse() =>
        Assert.False(SubtreeOfAnotherTreeSolution.IsSubtreeBySerializeThenKmpSearch(
            Tree(1, left: Tree(2)), Tree(3)));

    // 3 -> left: 4(left:1, right:2), right: 5
    private static BinaryTreeNode<int> Root() =>
        Tree(3, left: Tree(4, left: Tree(1), right: Tree(2)), right: Tree(5));

    // Same as Root(), but 4's right child (2) carries an extra leaf, so no node's
    // subtree is structurally identical to SubRoot() anymore.
    private static BinaryTreeNode<int> RootWithExtraLeaf() =>
        Tree(3, left: Tree(4, left: Tree(1), right: Tree(2, left: Tree(0))), right: Tree(5));

    // 4 -> left: 1, right: 2
    private static BinaryTreeNode<int> SubRoot() => Tree(4, left: Tree(1), right: Tree(2));

    private static BinaryTreeNode<int> Tree(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };
}
