using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReorderList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReorderList;

// Harness only. The single strategy is ReorderListSolution's - this file
// builds LeetCode's published examples as linked lists, reorders in place,
// and checks the resulting list's values.
public sealed partial class ReorderListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4], [1, 4, 2, 3] },
            { [1, 2, 3, 4, 5], [1, 5, 2, 4, 3] },
            { [1], [1] },
            { [1, 2], [1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReorderByReverseAndMergeInPlace_LeetCodeExamples_ReordersInPlace(int[] values, int[] expected)
    {
        var head = BuildList(values);

        ReorderListSolution.ReorderByReverseAndMergeInPlace(head);

        Assert.Equal(expected, ToArray(head));
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
