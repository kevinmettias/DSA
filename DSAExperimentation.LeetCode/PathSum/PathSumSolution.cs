using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.PathSum;

// LeetCode 112. Path Sum: does any root-to-leaf path sum to a target value?
internal static class PathSumSolution
{
    // The textbook answer: BCL-only recursion, subtracting each node's value from
    // the running target as it descends and stopping at the first accepting leaf
    // without ever materializing a path. Deliberately written without this repo's
    // primitives - it is the arm the path-enumeration strategy below has to
    // justify itself against. (Pre-migration this lived only in the benchmark,
    // as its untested [Benchmark(Baseline = true)] arm.)
    public static bool HasPathSumByRecursion(BinaryTreeNode<int>? node, int targetSum) =>
        node is not null && (node.Left is null && node.Right is null
            ? node.Value == targetSum
            : HasPathSumByRecursion(node.Left, targetSum - node.Value) ||
              HasPathSumByRecursion(node.Right, targetSum - node.Value));

    // This repo's own root-to-leaf path enumerator - Algorithms/Paths'
    // AllRootToLeafPaths, already exactly "every path from root to a leaf" - so the
    // puzzle reduces to "does any of them sum to target".
    public static bool HasPathSumByPathEnumeration(BinaryTreeNode<int>? root, int targetSum) =>
        AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root)
            .Any(path => path.Sum(n => n.Value) == targetSum);
}
