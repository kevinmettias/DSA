using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SameTree;

// LeetCode 100. Same Tree: two trees are the same exactly when every
// corresponding pair of nodes agrees on value and both subtrees recurse to the
// same conclusion - a missing node on one side with a present node on the other
// is an immediate mismatch, and two missing nodes are an immediate match.
//
// Two strategies, and only two: the recursive compare below, and the iterative
// compare above it, which carries the still-unvisited node pairs on an explicit stack
// instead of the call stack. Pre-migration the test's private helper and both of the
// old benchmark's [Benchmark] arms all called the recursive walk under different names,
// so only that walk needed naming; the iterative arm is genuinely new, not a rename of it.
internal static class SameTreeSolution
{
    // The explicit-stack counterpart to the recursion below: each still-unvisited pair of
    // corresponding nodes is pushed onto a Stack and popped in turn, so a deep tree cannot
    // grow the call stack. It makes the same decisions - a missing node on exactly one
    // side is a mismatch, two missing nodes are a match, and present nodes must agree on
    // value - so it must reach the same verdict the recursive arm does.
    public static bool IsSameByIterativeStackCompare(
        BinaryTreeNode<int>? firstTree, BinaryTreeNode<int>? secondTree)
    {
        var pending = new Stack<(BinaryTreeNode<int>? First, BinaryTreeNode<int>? Second)>();
        pending.Push((firstTree, secondTree));

        while (pending.Count > 0)
        {
            var (first, second) = pending.Pop();

            if (first is null || second is null)
            {
                if (first is not null || second is not null)
                {
                    return false;
                }

                continue;
            }

            if (first.Value != second.Value)
            {
                return false;
            }

            pending.Push((first.Left, second.Left));
            pending.Push((first.Right, second.Right));
        }

        return true;
    }

    public static bool IsSameByRecursiveCompare(BinaryTreeNode<int>? firstTree, BinaryTreeNode<int>? secondTree) =>
        firstTree is null || secondTree is null
            ? firstTree is null && secondTree is null
            : firstTree.Value == secondTree.Value &&
              IsSameByRecursiveCompare(firstTree.Left, secondTree.Left) &&
              IsSameByRecursiveCompare(firstTree.Right, secondTree.Right);
}
