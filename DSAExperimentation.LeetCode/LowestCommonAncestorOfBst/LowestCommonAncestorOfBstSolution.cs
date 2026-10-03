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
// metrics/paths.
internal static class LowestCommonAncestorOfBstSolution
{
    // The textbook arm the generic engine is measured against: descend from the root using the
    // BST ordering alone, going left while both targets sit below the current value and right
    // while both sit above it. It needs no parent links and no two-pass walk, which is exactly
    // what the ordering is worth - and the reason it cannot answer the same question on a tree
    // that is not ordered.
    public static BinaryTreeNode<int>? FindLcaByBstValueComparison(
        BinaryTreeNode<int> root, BinaryTreeNode<int> first, BinaryTreeNode<int> second)
    {
        var current = root;

        while (true)
        {
            var goLeft = first.Value < current.Value && second.Value < current.Value;

            if (goLeft || (first.Value > current.Value && second.Value > current.Value))
            {
                var next = goLeft ? current.Left : current.Right;

                if (next is null)
                {
                    return null;
                }

                current = next;
                continue;
            }

            return current;
        }
    }

    public static BinaryTreeNode<int>? FindLcaByAncestryWalk(
        BinaryTreeNode<int> root, BinaryTreeNode<int> first, BinaryTreeNode<int> second) =>
        LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>>(
            root, first, second);
}
