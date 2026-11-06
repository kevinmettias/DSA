using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode;

// LeetCode states a tree or a list as a flat array, and every problem taking one
// used to re-implement that translation in its own test file. It is the same
// translation every time, so it lives once here - which is also what lets a
// test's examples read like LeetCode's own example text rather than like tree
// construction code. FromBinaryTree is the same notation back out, so an expected
// tree can be LeetCode's published output array.
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

    private static BinaryTreeNode<int>? TakeChild(
        int?[] levelOrder, ref int index, Queue<BinaryTreeNode<int>> pending)
    {
        if (TakeNextValue(levelOrder, ref index) is not { } childValue)
        {
            return null;
        }

        return EnqueueChild(childValue, pending);
    }

    // The next array entry, or null once the array runs out. The index advances only
    // when a value was there to read, so an absent child leaves it untouched.
    private static int? TakeNextValue(int?[] levelOrder, ref int index)
    {
        if (index >= levelOrder.Length)
        {
            return null;
        }

        return levelOrder[index++];
    }

    // Materialize the child and queue it, so a later iteration takes its own children.
    private static BinaryTreeNode<int>? EnqueueChild(
        int childValue, Queue<BinaryTreeNode<int>> pending)
    {
        var child = new BinaryTreeNode<int>(childValue);
        pending.Enqueue(child);

        return child;
    }

    // The same shape back out, exactly as LeetCode prints a tree answer: level order,
    // null for an absent child of a present node, nothing for the children of an
    // absent one, and the trailing nulls of the last level trimmed. An expected tree
    // can therefore be stated as LeetCode's published output array and compared
    // with one Assert.Equal, which a field-by-field walk would need a helper for.
    public static int?[] FromBinaryTree(BinaryTreeNode<int>? root)
    {
        var values = new List<int?>();
        var pending = new Queue<BinaryTreeNode<int>?>();
        pending.Enqueue(root);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            values.Add(node?.Value);

            if (node is not null)
            {
                pending.Enqueue(node.Left);
                pending.Enqueue(node.Right);
            }
        }

        var length = values.Count;

        while (length > 0 && values[length - 1] is null)
        {
            length--;
        }

        return [.. values.Take(length)];
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

    // The cycle LeetCode's judge builds from pos and never passes to the function: the list's tail links back
    // to the node at index pos, and a pos of -1, or one past the list, leaves the list as it is.
    public static SinglyLinkedListNode<int>? WithCycleAt(SinglyLinkedListNode<int>? head, int pos)
    {
        if (NodeAt(head, pos) is { } entry)
        {
            TailFrom(entry).Next = entry;
        }

        return head;
    }

    // The node at index pos, or null for a negative pos or one past the list.
    private static SinglyLinkedListNode<int>? NodeAt(SinglyLinkedListNode<int>? head, int pos)
    {
        if (pos < 0)
        {
            return null;
        }

        var node = head;

        for (var index = 0; index < pos && node is not null; index++)
        {
            node = node.Next;
        }

        return node;
    }

    private static SinglyLinkedListNode<int> TailFrom(SinglyLinkedListNode<int> node)
    {
        var tail = node;

        while (tail.Next is not null)
        {
            tail = tail.Next;
        }

        return tail;
    }

    // The node of a tree that LeetCode names by its value, as its judge does when a function takes nodes of
    // the tree it is handed, whose values such questions keep unique. The judge names only a node the tree
    // holds, so a value no node holds is a case that breaks the question's promise.
    public static BinaryTreeNode<int> NodeWithValue(BinaryTreeNode<int>? root, int value)
    {
        var pending = new Stack<BinaryTreeNode<int>>();

        if (root is not null)
        {
            pending.Push(root);
        }

        while (pending.TryPop(out var node))
        {
            if (node.Value == value)
            {
                return node;
            }

            PushChildren(pending, node);
        }

        throw new ArgumentException($"No node of the tree holds {value}.", nameof(value));
    }

    // The node of a list that LeetCode names by its value, as Delete Node in a Linked List's judge does,
    // whose values that question keeps unique; as for a tree, the judge names only a node the list holds.
    public static SinglyLinkedListNode<int> NodeWithValue(SinglyLinkedListNode<int>? head, int value)
    {
        for (var node = head; node is not null; node = node.Next)
        {
            if (node.Value == value)
            {
                return node;
            }
        }

        throw new ArgumentException($"No node of the list holds {value}.", nameof(value));
    }

    private static void PushChildren(Stack<BinaryTreeNode<int>> pending, BinaryTreeNode<int> node)
    {
        if (node.Left is not null)
        {
            pending.Push(node.Left);
        }

        if (node.Right is not null)
        {
            pending.Push(node.Right);
        }
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
}
