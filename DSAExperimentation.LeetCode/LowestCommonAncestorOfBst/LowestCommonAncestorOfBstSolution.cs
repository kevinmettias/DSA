using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LowestCommonAncestorOfBst;

// LeetCode 235. Lowest Common Ancestor of a Binary Search Tree: given a BST's root
// and two of its nodes, return their lowest common ancestor.
//
// The BST ordering is never actually needed to answer this - Algorithms.Ancestry's
// generic tree LCA engine composes directly over a BinaryTreeNode<int>'s Left/Right
// shape with zero BST-specific code of its own, the same "generic engines are a
// free win" payoff phase 1 of this repo's tree work already found for traversal/
// metrics/paths. Only one strategy exists in this repo's coverage today.
internal static class LowestCommonAncestorOfBstSolution
{
    public static BinaryTreeNode<int>? FindLcaByAncestryWalk(
        BinaryTreeNode<int> root, BinaryTreeNode<int> first, BinaryTreeNode<int> second) =>
        LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(
            root, first, second);
}
