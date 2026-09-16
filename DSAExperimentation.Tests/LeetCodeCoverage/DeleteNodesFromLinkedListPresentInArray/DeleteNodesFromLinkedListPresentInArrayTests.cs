using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteNodesFromLinkedListPresentInArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteNodesFromLinkedListPresentInArray;

// Harness only. Both strategies are
// DeleteNodesFromLinkedListPresentInArraySolution's - this file pins them to
// LeetCode's published examples.
public sealed partial class DeleteNodesFromLinkedListPresentInArrayTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3], [1, 2, 3, 4, 5], [4, 5] },
            { [1], [1, 2, 1, 2, 1, 2], [2, 2, 2] },
            { [5], [1, 2, 3, 4], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifiedListByArrayScan_LeetCodeExamples_RemovesNodesWhoseValueIsInNums(
        int[] nums, int[] headValues, int[] expected)
    {
        var list = DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListByArrayScan(nums, BuildList(headValues));
        var actual = ToArray(list);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModifiedListBySetFilter_LeetCodeExamples_RemovesNodesWhoseValueIsInNums(
        int[] nums, int[] headValues, int[] expected)
    {
        var list = DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListBySetFilter(nums, BuildList(headValues));
        var actual = ToArray(list);
        Assert.Equal(expected, actual);
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
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
