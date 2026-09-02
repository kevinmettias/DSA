using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LowestCommonAncestorOfABinaryTree;

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
internal static class LowestCommonAncestorOfABinaryTreeSolution
{
    public static BinaryTreeNode<int>? FindLcaByAncestryWalk(
        BinaryTreeNode<int> root, BinaryTreeNode<int> p, BinaryTreeNode<int> q) =>
        LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(
            root, p, q);
}
