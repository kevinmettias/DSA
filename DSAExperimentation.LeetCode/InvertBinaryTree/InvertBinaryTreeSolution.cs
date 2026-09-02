using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.InvertBinaryTree;

// LeetCode 226. Invert Binary Tree: swap every node's left and right subtree so
// the whole tree becomes its own mirror image.
//
// There is exactly one strategy: pre-migration the test carried a private
// recursive swap, and the benchmark's two [Benchmark] arms were unwired stubs
// that each just returned the literal 1, never calling into any algorithm at
// all - so nothing needed reconciling beyond naming this walk once.
internal static class InvertBinaryTreeSolution
{
    public static BinaryTreeNode<int>? InvertByRecursiveSwap(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return null;
        }

        (root.Left, root.Right) = (InvertByRecursiveSwap(root.Right), InvertByRecursiveSwap(root.Left));
        return root;
    }
}
