using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode;

// LeetCode's quad tree, as Construct Quad Tree and Logical OR of Two Binary Grids write it: in level order,
// each node as [isLeaf, val] with 1 for true, and after the root every node's four children in turn - top
// left, top right, bottom left, bottom right - null where there is none, as a leaf's four always are, with the
// nulls that trail the last node dropped.
internal static class LeetCodeQuadTrees
{
    private const int Truth = 1;

    public static QuadTreeNode? From(int[]?[] levelOrder) => LevelOrderReading.TreeOf(levelOrder);

    public static int[]?[] LevelOrderOf(QuadTreeNode? root)
    {
        var entries = new List<int[]?>();
        var pending = new Queue<QuadTreeNode?>();
        pending.Enqueue(root);

        while (pending.TryDequeue(out var node))
        {
            entries.Add(EntryOf(node));
            EnqueueChildren(pending, node);
        }

        return WithoutTrailingNulls(entries);
    }

    private static int[]? EntryOf(QuadTreeNode? node)
    {
        if (node is null)
        {
            return null;
        }

        return [node.IsLeaf ? Truth : 0, node.Val ? Truth : 0];
    }

    // A node's four children, in LeetCode's order; a missing node has none to enqueue.
    private static void EnqueueChildren(Queue<QuadTreeNode?> pending, QuadTreeNode? node)
    {
        if (node is null)
        {
            return;
        }

        foreach (var child in ChildrenOf(node))
        {
            pending.Enqueue(child);
        }
    }

    private static int[]?[] WithoutTrailingNulls(List<int[]?> entries)
    {
        var kept = entries.Count;

        while (kept > 0 && entries[kept - 1] is null)
        {
            kept--;
        }

        return [.. entries.Take(kept)];
    }

    // Whether two quad trees stand for the same grid as LeetCode compares them: the same leaves in the same
    // places, an inner node's val aside, since its judge accepts either value there ("you can assign the val to
    // True or False when isLeaf is False, and both are accepted in the answer").
    public static bool IsSameTree(QuadTreeNode? first, QuadTreeNode? second)
    {
        if (first is null || second is null)
        {
            return first is null && second is null;
        }

        if (first.IsLeaf || second.IsLeaf)
        {
            return first.IsLeaf == second.IsLeaf && first.Val == second.Val;
        }

        var secondChildren = ChildrenOf(second);

        return ChildrenOf(first).Zip(secondChildren).All(pair => IsSameTree(pair.First, pair.Second));
    }

    private static QuadTreeNode?[] ChildrenOf(QuadTreeNode node) => [node.TopLeft, node.TopRight, node.BottomLeft, node.BottomRight];

    // A tree's children are fixed when it is built, so reading collects each node's child entries first and
    // builds the tree from its leaves up.
    private static class LevelOrderReading
    {
        private const int IsLeafSlot = 0;
        private const int ValueSlot = 1;
        private const int ChildCount = 4;

        public static QuadTreeNode? TreeOf(int[]?[] levelOrder)
        {
            var children = ChildEntriesOf(levelOrder);

            return BuildAt(levelOrder, children, 0);
        }

        // For each entry, the entries of its four children: every entry that is a node takes the next four
        // from the order in turn.
        private static int[][] ChildEntriesOf(int[]?[] levelOrder)
        {
            var children = new int[levelOrder.Length][];
            var next = 1;

            for (var entry = 0; entry < levelOrder.Length; entry++)
            {
                if (levelOrder[entry] is not null)
                {
                    children[entry] = [.. Enumerable.Range(next, ChildCount)];
                    next += ChildCount;
                }
            }

            return children;
        }

        // The node at an entry, built over its children, or null for a null entry or one past the end.
        private static QuadTreeNode? BuildAt(int[]?[] levelOrder, int[][] children, int entry)
        {
            if (entry >= levelOrder.Length || levelOrder[entry] is not { } fields)
            {
                return null;
            }

            // Every node's entry took four child entries.
            if (children[entry] is not [var topLeft, var topRight, var bottomLeft, var bottomRight])
            {
                return null;
            }

            return new QuadTreeNode(fields[ValueSlot] == Truth, fields[IsLeafSlot] == Truth)
            {
                TopLeft = BuildAt(levelOrder, children, topLeft),
                TopRight = BuildAt(levelOrder, children, topRight),
                BottomLeft = BuildAt(levelOrder, children, bottomLeft),
                BottomRight = BuildAt(levelOrder, children, bottomRight),
            };
        }
    }
}
