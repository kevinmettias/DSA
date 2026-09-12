using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.InsertIntoABinarySearchTree;

// LeetCode 701. Insert into a Binary Search Tree: given the root of a BST built
// from a sequence of existing values, plus a new value, return the root of a
// BST that also contains the new value. LeetCode accepts ANY valid resulting
// BST, so correctness only requires the returned tree to stay BST-ordered and
// to contain every original value plus the new one - not that its shape match
// the other strategy's.
internal static class InsertIntoABinarySearchTreeSolution
{
    private const int MidpointDivisor = 2;

    // The naive approach many first solutions reach for: collect the existing
    // tree's values via an in-order walk (built fresh from the same values
    // here, so the walk is folded into construction), fold the new value into
    // the sorted result, and rebuild a whole balanced tree from scratch -
    // touching every node on every insert. Deliberately BCL-only inside: the
    // point is contrasting against this repo's own persistent BST below.
    public static BinaryTreeNode<int>? InsertByCollectSortRebuild(IEnumerable<int> existingValues, int newValue)
    {
        var root = BuildManual(existingValues);

        var sorted = new List<int>();
        CollectInOrder(root, sorted);

        var insertAt = sorted.BinarySearch(newValue);
        sorted.Insert(insertAt < 0 ? ~insertAt : insertAt, newValue);

        return BuildBalanced(sorted, 0, sorted.Count - 1);
    }

    // This repo's own BinarySearchTree<TValue>.Insert: walks only the O(h)
    // root-to-empty-slot path and attaches one new leaf, never touching an
    // unrelated node.
    public static BinaryTreeNode<int>? InsertByBstInsert(IEnumerable<int> existingValues, int newValue)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in existingValues)
        {
            tree.Insert(value);
        }

        tree.Insert(newValue);
        return tree.Root;
    }

    private static BinaryTreeNode<int>? BuildManual(IEnumerable<int> values)
    {
        BinaryTreeNode<int>? root = null;

        foreach (var value in values)
        {
            if (root is null)
            {
                root = new BinaryTreeNode<int>(value);
                continue;
            }

            InsertManual(root, value);
        }

        return root;
    }

    private static void InsertManual(BinaryTreeNode<int> root, int value)
    {
        var node = root;

        while (true)
        {
            if (value < node.Value)
            {
                if (node.Left is null)
                {
                    node.Left = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right is null)
                {
                    node.Right = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Right;
            }
        }
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node.Value);
        CollectInOrder(node.Right, values);
    }

    private static BinaryTreeNode<int>? BuildBalanced(List<int> values, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / MidpointDivisor);

        return new BinaryTreeNode<int>(values[mid])
        {
            Left = BuildBalanced(values, low, mid - 1),
            Right = BuildBalanced(values, mid + 1, high),
        };
    }
}
