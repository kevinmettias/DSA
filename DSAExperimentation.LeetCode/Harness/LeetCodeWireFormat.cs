using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness;

// LeetCode states a tree or a list as a flat array, and every problem taking one
// used to re-implement that translation in its own test file. It is the same
// translation every time, so it lives once here - which is also what lets a
// registration's Case read like LeetCode's own example text rather than like tree
// construction code.
internal static class LeetCodeWireFormat
{
    // LeetCode's level-order array, where null marks an absent child and the
    // children of an absent node are omitted entirely rather than padded - so the
    // array is consumed by a queue of real nodes, not indexed by 2i+1/2i+2.
    public static BinaryTreeNode<int>? ToBinaryTree(int?[] levelOrder)
    {
        if (levelOrder.Length == 0 || levelOrder[0] is not { } rootValue)
        {
            return null;
        }

        var root = new BinaryTreeNode<int>(rootValue);
        var pending = new Queue<BinaryTreeNode<int>>();
        pending.Enqueue(root);
        var index = 1;

        while (pending.Count > 0 && index < levelOrder.Length)
        {
            var parent = pending.Dequeue();
            parent.Left = TakeChild(levelOrder, ref index, pending);
            parent.Right = TakeChild(levelOrder, ref index, pending);
        }

        return root;
    }

    public static SinglyLinkedListNode<int>? ToLinkedList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;

        for (var index = values.Length - 1; index >= 0; index--)
        {
            head = new SinglyLinkedListNode<int>(values[index]) { Next = head };
        }

        return head;
    }

    public static int[] FromLinkedList(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    private static BinaryTreeNode<int>? TakeChild(
        int?[] levelOrder, ref int index, Queue<BinaryTreeNode<int>> pending)
    {
        if (index >= levelOrder.Length)
        {
            return null;
        }

        var value = levelOrder[index++];

        if (value is not { } childValue)
        {
            return null;
        }

        var child = new BinaryTreeNode<int>(childValue);
        pending.Enqueue(child);

        return child;
    }
}
