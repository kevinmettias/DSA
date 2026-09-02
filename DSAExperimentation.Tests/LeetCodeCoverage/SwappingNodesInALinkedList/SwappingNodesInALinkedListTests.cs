using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwappingNodesInALinkedList;

// LeetCode 1721. Swapping Nodes in a Linked List: one pass over this repo's mutable
// SinglyLinkedListNode<T> to reach the kth node from the front, then a fast/slow pair of
// node references walked together so the slow reference lands on the kth node from the
// end the moment fast runs out - swap the two nodes' Values in place, the same "rewrite
// Value, not the pointers" shape SwapNodesInPairsTests uses for its own node primitive.
public sealed partial class SwappingNodesInALinkedListTests
{
    [Fact]
    public void SwapNodes_ClassicExample_SwapsKthFromFrontAndEnd()
    {
        var head = BuildList([1, 2, 3, 4, 5]);

        var swapped = SwapNodes(head, 2);
        var result = ToArray(swapped);

        Assert.Equal([1, 4, 3, 2, 5], result);
    }

    [Fact]
    public void SwapNodes_KEqualsOne_SwapsFirstAndLastNodes()
    {
        var head = BuildList([7, 9, 6, 6, 7, 8, 3, 0, 9, 5]);

        var swapped = SwapNodes(head, 1);
        var result = ToArray(swapped);

        Assert.Equal([5, 9, 6, 6, 7, 8, 3, 0, 9, 7], result);
    }

    [Fact]
    public void SwapNodes_SingleNodeList_LeavesListUnchanged()
    {
        var head = BuildList([42]);

        var swapped = SwapNodes(head, 1);
        var result = ToArray(swapped);

        Assert.Equal([42], result);
    }

    private static SinglyLinkedListNode<int>? SwapNodes(SinglyLinkedListNode<int>? head, int k)
    {
        var front = head;
        for (var i = 1; i < k; i++)
        {
            front = front!.Next;
        }

        var end = head;
        var runner = front;
        while (runner!.Next is not null)
        {
            runner = runner.Next;
            end = end!.Next;
        }

        (front!.Value, end!.Value) = (end.Value, front.Value);
        return head;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
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
