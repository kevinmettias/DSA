using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.LowestCommonAncestorOfABinaryTree;

// LeetCode 236. Lowest Common Ancestor of a Binary Tree: given a binary tree's root
// and two of its nodes, return their lowest common ancestor.
//
// Structurally identical to LC 235's LowestCommonAncestorOfBstSolution - the tree
// here just isn't a BST, and the generic ancestry-walk engine never used the BST
// ordering anyway (Algorithms.Ancestry.LowestCommonAncestor.Find is generic over
// any ITreeTopology). Only one strategy exists in this repo's coverage today: the
// reference-equality recursive walk that used to be a private helper duplicated
// into the test. The benchmark was a compile-smoke placeholder (Baseline() => 1,
// PrimitiveComposed() => 1) that took no input and computed nothing, so there was
// no second arm to reconcile this against.
[LeetCodeQuestion("lowest-common-ancestor-of-a-binary-tree", LeetCodeAnswerOrder.AsWritten, Promise = typeof(LowestCommonAncestorOfABinaryTreePromise))]
internal static class LowestCommonAncestorOfABinaryTreeSolution
{
    // LeetCode's shape (ARCHITECTURE 17.4): its signature names p and q by their values, which its judge turns
    // into the tree's nodes, and it prints the ancestor a function answers with as that node's value.
    public static int FindLcaByAncestryWalk(BinaryTreeNode<int> root, int firstValue, int secondValue)
    {
        var firstNode = LeetCodeWireFormat.NodeWithValue(root, firstValue);
        var secondNode = LeetCodeWireFormat.NodeWithValue(root, secondValue);
        var ancestor = FindLcaByAncestryWalk(root, firstNode, secondNode)
            ?? throw new InvalidOperationException("Two nodes of one tree always share its root as an ancestor.");

        return ancestor.Value;
    }

    // `firstNode` and `secondNode` are the two nodes whose lowest common ancestor is asked
    // for; the engine below never consults an ordering, so neither has to precede the other.
    public static BinaryTreeNode<int>? FindLcaByAncestryWalk(
        BinaryTreeNode<int> root, BinaryTreeNode<int> firstNode, BinaryTreeNode<int> secondNode) =>
        LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>>(
            root, firstNode, secondNode);
}
