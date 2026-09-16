using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.TwoSumIVInputIsABST;

// LeetCode 653. Two Sum IV - Input is a BST: given the root of a BST and a target
// k, decide whether two distinct node values sum to k.
//
// Both strategies take LeetCode's own input shape directly - a BinaryTreeNode<int>
// root - so there is no separate hoisted-input overload: the tree already IS the
// prepared input, built once in a benchmark's [GlobalSetup] or once per test case.
internal static class TwoSumIVInputIsABSTSolution
{
    // The textbook answer: collect every value with a plain traversal, then a
    // nested O(n^2) pair scan. Deliberately BCL-only past the traversal - the arm
    // the one-pass DFS below has to justify itself against.
    public static bool HasTargetPairByNestedPairScan(BinaryTreeNode<int>? root, int target)
    {
        var values = new List<int>();
        Collect(root, values);

        for (var i = 0; i < values.Count; i++)
        {
            for (var j = i + 1; j < values.Count; j++)
            {
                if (values[i] + values[j] == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // A single DFS that checks each node's complement against a Set<int> of values
    // already seen - the same one-pass Set/HashMap idiom TwoSum and
    // ContainsDuplicate use, walking tree edges instead of an array index.
    public static bool HasTargetPairByDepthFirstSetLookup(BinaryTreeNode<int>? root, int target) =>
        HasTargetPairByDepthFirstSetLookup(root, target, new Set<int>());

    private static bool HasTargetPairByDepthFirstSetLookup(BinaryTreeNode<int>? node, int target, Set<int> seen)
    {
        if (node is null)
        {
            return false;
        }

        if (seen.Has(target - node.Value))
        {
            return true;
        }

        seen.TryAdd(node.Value);
        return HasTargetPairByDepthFirstSetLookup(node.Left, target, seen) ||
               HasTargetPairByDepthFirstSetLookup(node.Right, target, seen);
    }

    private static void Collect(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        Collect(node.Left, values);
        Collect(node.Right, values);
    }
}
