using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

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
    // Even values are twice a draw from [-MaxHalfValue, MaxHalfValue].
    private const int MaxHalfValue = 25;

    // The target is one more than twice a draw from [-MaxHalfTarget, MaxHalfTarget).
    private const int MaxHalfTarget = 200;

    // In a gapless level order, node i's children sit at ChildrenPerNode * i + 1 and the slot after it.
    private const int ChildrenPerNode = 2;

    public static (BinaryTreeNode<int> Root, int TargetSum) Build(int nodeCount, int matchingPaths, Random random)
    {
        var levelOrder = Array.ConvertAll(
            SeededDraws.Values(nodeCount, -MaxHalfValue, MaxHalfValue + 1, random),
            half => ChildrenPerNode * half);
        var targetSum = (ChildrenPerNode * random.Next(-MaxHalfTarget, MaxHalfTarget)) + 1;

        foreach (var leaf in LeavesLeftToRight(nodeCount).TakeLast(matchingPaths))
        {
            levelOrder[leaf] = targetSum - AncestorSum(levelOrder, leaf);
        }

        return (LeetCodeWireFormat.ToBinaryTree(Array.ConvertAll(levelOrder, value => (int?)value))!, targetSum);
    }

    // A left-first depth-first walk over the level-order indices, keeping the ones with no children.
    private static List<int> LeavesLeftToRight(int nodeCount)
    {
        var leaves = new List<int>();
        var pending = new Stack<int>();
        pending.Push(0);

        while (pending.Count > 0)
        {
            var node = pending.Pop();
            var left = (ChildrenPerNode * node) + 1;

            if (left >= nodeCount)
            {
                leaves.Add(node);
                continue;
            }

            if (left + 1 < nodeCount)
            {
                pending.Push(left + 1);
            }

            pending.Push(left);
        }

        return leaves;
    }

    private static int AncestorSum(int[] levelOrder, int node)
    {
        var sum = 0;
        var current = node;

        while (current > 0)
        {
            current = (current - 1) / ChildrenPerNode;
            sum += levelOrder[current];
        }

        return sum;
    }
}
