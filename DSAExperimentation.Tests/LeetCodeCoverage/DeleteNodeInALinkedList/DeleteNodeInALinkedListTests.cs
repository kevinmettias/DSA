using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteNodeInALinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteNodeInALinkedList;

// Harness only. The single strategy is DeleteNodeInALinkedListSolution's - this
// file builds LeetCode's published examples as linked lists, calls the solution on
// the node at the given position (LeetCode's signature never passes a head
// reference), then reads the resulting list back from head to verify.
public sealed partial class DeleteNodeInALinkedListTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [4, 5, 1, 9], 1, [4, 1, 9] }, // LeetCode's example 1: delete the second node (value 5)
            { [4, 5, 1, 9], 2, [4, 5, 9] }, // LeetCode's example 2: delete the third node (value 1)
            { [5, 1, 9], 0, [1, 9] }, // the original coverage test's case: delete the head node
            { [1, 2], 0, [2] }, // two-node list, delete the head
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteByNextValueCopy_LeetCodeExamples_RemovesTheGivenNode(
        int[] values, int nodeIndex, int[] expected)
    {
        var head = BuildList(values);
        var target = NodeAt(head, nodeIndex);

        DeleteNodeInALinkedListSolution.DeleteByNextValueCopy(target);

        Assert.Equal(expected, ToArray(head));
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
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

    private static SinglyLinkedListNode<int> NodeAt(SinglyLinkedListNode<int> head, int index)
    {
        var node = head;

        // Every Examples row's nodeIndex stays below the length of the list BuildList
        // builds for it, so this walk never steps off the tail.
        for (var i = 0; i < index; i++)
        {
            node = node.Next
                ?? throw new InvalidOperationException(
                    $"every Examples row's nodeIndex is below its list's length, but this walk was asked for index {index}");
        }

        return node;
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
