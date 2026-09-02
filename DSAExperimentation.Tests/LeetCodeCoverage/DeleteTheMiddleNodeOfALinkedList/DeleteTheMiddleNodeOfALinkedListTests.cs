using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteTheMiddleNodeOfALinkedList;

// LeetCode 2095. Delete the Middle Node of a Linked List: the same slow/fast
// two-pointer walk CycleDetection.cs already uses over SinglyLinkedListNode<T>.Next,
// applied here to find the predecessor of the middle node (index n/2, 0-indexed)
// instead of a cycle's meeting point - slow trails one step behind fast's
// double-speed advance, so slow lands on the middle exactly when fast runs off the
// end, and a trailing prev pointer is what makes the unlink an O(1) splice once it
// gets there.
public sealed partial class DeleteTheMiddleNodeOfALinkedListTests
{
    [Fact]
    public void DeleteMiddle_OddLength_RemovesTheSingleMiddleNode()
    {
        var head = Build([1, 3, 4, 7, 1, 2, 6]);

        var result = DeleteMiddle(head);

        Assert.Equal([1, 3, 4, 1, 2, 6], ToArray(result));
    }

    [Fact]
    public void DeleteMiddle_EvenLength_RemovesTheSecondOfTheTwoMiddleNodes()
    {
        var head = Build([1, 2, 3, 4]);

        var result = DeleteMiddle(head);

        Assert.Equal([1, 2, 4], ToArray(result));
    }

    [Fact]
    public void DeleteMiddle_TwoNodes_LeavesOnlyTheHead()
    {
        var head = Build([2, 1]);

        var result = DeleteMiddle(head);

        Assert.Equal([2], ToArray(result));
    }

    [Fact]
    public void DeleteMiddle_SingleNode_ReturnsEmptyList()
    {
        var head = Build([1]);

        var result = DeleteMiddle(head);

        Assert.Null(result);
    }

    private static SinglyLinkedListNode<int>? DeleteMiddle(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return null;
        }

        var prev = head;
        var slow = head;
        var fast = head;

        while (fast is not null && fast.Next is not null)
        {
            prev = slow;
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        prev!.Next = slow!.Next;
        return head;
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
