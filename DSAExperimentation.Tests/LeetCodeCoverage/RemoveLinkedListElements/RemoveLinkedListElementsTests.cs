using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveLinkedListElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveLinkedListElements;

// Harness only. Both strategies are RemoveLinkedListElementsSolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values, including the case where val matches the head node itself.
public sealed class RemoveLinkedListElementsTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 6, 3, 4, 5, 6], 6, [1, 2, 3, 4, 5] }, // LC's example 1
            { [], 1, [] }, // LC's example 2
            { [7, 7, 7, 7], 7, [] }, // LC's example 3
            { [7, 1, 7, 2, 7], 7, [1, 2] }, // matches at the head too
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveElementsByArrayRebuild_LeetCodeExamples_RemovesMatchingValues(
        int[] values, int val, int[] expected)
    {
        var removed = RemoveLinkedListElementsSolution.RemoveElementsByArrayRebuild(BuildList(values), val);
        var actual = ToArray(removed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveElementsByDummyHeadSplice_LeetCodeExamples_RemovesMatchingValues(
        int[] values, int val, int[] expected)
    {
        var removed = RemoveLinkedListElementsSolution.RemoveElementsByDummyHeadSplice(BuildList(values), val);
        var actual = ToArray(removed);

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
