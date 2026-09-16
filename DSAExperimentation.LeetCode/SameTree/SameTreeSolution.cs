using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SameTree;

// LeetCode 100. Same Tree: two trees are the same exactly when every
// corresponding pair of nodes agrees on value and both subtrees recurse to the
// same conclusion - a missing node on one side with a present node on the other
// is an immediate mismatch, and two missing nodes are an immediate match.
//
// There is exactly one strategy: pre-migration the test's private helper and
// both of the benchmark's [Benchmark] arms all called this identical recursive
// walk under different names, so nothing needed reconciling beyond naming it once
// and deleting the redundant second arm.
internal static class SameTreeSolution
{
    public static bool IsSameByRecursiveCompare(BinaryTreeNode<int>? firstTree, BinaryTreeNode<int>? secondTree) =>
        firstTree is null || secondTree is null
            ? firstTree is null && secondTree is null
            : firstTree.Value == secondTree.Value &&
              IsSameByRecursiveCompare(firstTree.Left, secondTree.Left) &&
              IsSameByRecursiveCompare(firstTree.Right, secondTree.Right);
}
