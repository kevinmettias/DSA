using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 112 and LC 113: a complete tree - LeetCode's
// level-order array with no gaps - in which exactly the last matchingPaths leaves,
// in left-to-right order, end a root-to-leaf path summing to the target, and no other
// leaf does. Every node value is even except those leaves', each of which is odd and
// chosen to bring its own path to the odd target; every other path sums to an even
// number and so cannot match. Putting the matches last means a search that stops at
// its first match still visits every other leaf first.
//
// Values stay within LC's [-1000, 1000] at its 5,000-node cap: an even value is at
// most 50 in magnitude, so the at most 12 ancestors of a leaf sum to at most 600, and
// the target is at most 399 in magnitude, so a planted leaf needs at most 999.
internal static class PathSumWorkloads
{
    // Even values are twice a draw from [-MaxHalfValue, MaxHalfValue]: the draw is the value halved.
    private const int MaxHalfValue = 25;

    // The target is one more than twice a draw from [-MaxHalfTarget, MaxHalfTarget).
    private const int MaxHalfTarget = 200;

    public static (BinaryTreeNode<int> Root, int TargetSum) Build(int nodeCount, int matchingPaths, Random random)
    {
        var halves = SeededDraws.Values(nodeCount, -MaxHalfValue, MaxHalfValue + 1, random);
        var levelOrder = Array.ConvertAll(halves, half => AlgorithmConstants.HalvingFactor * half);
        var targetSum = (AlgorithmConstants.HalvingFactor * random.Next(-MaxHalfTarget, MaxHalfTarget)) + 1;

        foreach (var leaf in LeavesLeftToRight(nodeCount).TakeLast(matchingPaths))
        {
            levelOrder[leaf] = targetSum - AncestorSum(levelOrder, leaf);
        }

        return (BinaryTrees.Complete(levelOrder), targetSum);
    }

    // A left-first depth-first walk over the level-order indices, keeping the ones with no children.
    // In a gapless level order, node i's children sit at BranchingFactor * i + 1 and the slot after it.
    private static List<int> LeavesLeftToRight(int nodeCount)
    {
        var leaves = new List<int>();
        var pending = new Stack<int>();
        pending.Push(0);

        while (pending.Count > 0)
        {
            var node = pending.Pop();
            var left = (AlgorithmConstants.BranchingFactor * node) + 1;

            if (left >= nodeCount)
            {
                leaves.Add(node);
                continue;
            }

            PushChildren(pending, left, nodeCount);
        }

        return leaves;
    }

    // The right child goes on first, so the left one - and its whole subtree - is walked before it.
    private static void PushChildren(Stack<int> pending, int left, int nodeCount)
    {
        if (left + 1 < nodeCount)
        {
            pending.Push(left + 1);
        }

        pending.Push(left);
    }

    private static int AncestorSum(int[] levelOrder, int node)
    {
        var sum = 0;
        var current = node;

        while (current > 0)
        {
            current = (current - 1) / AlgorithmConstants.BranchingFactor;
            sum += levelOrder[current];
        }

        return sum;
    }
}
