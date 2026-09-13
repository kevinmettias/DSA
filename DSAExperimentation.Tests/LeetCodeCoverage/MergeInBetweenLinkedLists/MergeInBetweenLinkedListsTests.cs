using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeInBetweenLinkedLists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeInBetweenLinkedLists;

// Harness only. Both strategies are MergeInBetweenLinkedListsSolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values. Each theory builds its own lists because the splice strategy
// rewires the nodes it is handed.
public sealed class MergeInBetweenLinkedListsTests
{
    public static TheoryData<int[], int, int, int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1.
            { [0, 1, 2, 3, 4, 5], 3, 4, [1000000, 1000001, 1000002], [0, 1, 2, 1000000, 1000001, 1000002, 5] },

            // LeetCode example 2.
            {
                [0, 1, 2, 3, 4, 5, 6], 2, 5, [1000000, 1000001, 1000002, 1000003, 1000004],
                [0, 1, 1000000, 1000001, 1000002, 1000003, 1000004, 6]
            },

            // Smallest window the constraints allow: a == b, one node removed.
            { [0, 1, 2, 3, 4], 1, 1, [9, 8], [0, 9, 8, 2, 3, 4] },

            // Widest window the constraints allow: everything but the first and last node.
            { [0, 1, 2, 3, 4], 1, 3, [7], [0, 7, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeInBetweenByArrayRebuild_LeetCodeExamples_SplicesList2InPlaceOfRemovedRange(
        int[] list1, int a, int b, int[] list2, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(MergeInBetweenLinkedListsSolution.MergeInBetweenByArrayRebuild(
                BuildList(list1), a, b, BuildList(list2))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeInBetweenByPointerSplice_LeetCodeExamples_SplicesList2InPlaceOfRemovedRange(
        int[] list1, int a, int b, int[] list2, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(MergeInBetweenLinkedListsSolution.MergeInBetweenByPointerSplice(
                BuildList(list1), a, b, BuildList(list2))));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next) values.Add(node.Value);
        return values.ToArray();
    }
}
