using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseNodesInKGroup;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseNodesInKGroup;

// Harness only. Both strategies are ReverseNodesInKGroupSolution's - this file
// pins them to LeetCode's published examples, including groupSize = 1 (a no-op) and a
// single-node list (always a short final group).
public sealed class ReverseNodesInKGroupTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [2, 1, 4, 3, 5] },
            { [1, 2, 3, 4, 5], 3, [3, 2, 1, 4, 5] },
            { [1, 2, 3, 4, 5], 1, [1, 2, 3, 4, 5] },
            { [1], 1, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseKGroupByPointerReversal_LeetCodeExamples_ReversesOnlyCompleteGroups(
        int[] values, int groupSize, int[] expected)
    {
        var reversed = ReverseNodesInKGroupSolution.ReverseKGroupByPointerReversal(BuildList(values), groupSize);
        var actual = ToArray(reversed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseKGroupByArrayReverse_LeetCodeExamples_ReversesOnlyCompleteGroups(
        int[] values, int groupSize, int[] expected)
    {
        var reversed = ReverseNodesInKGroupSolution.ReverseKGroupByArrayReverse(BuildList(values), groupSize);
        var actual = ToArray(reversed);

        Assert.Equal(expected, actual);
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
