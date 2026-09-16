using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeInBetweenLinkedLists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeInBetweenLinkedLists;

// Harness only. Both strategies are MergeInBetweenLinkedListsSolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values. Each theory builds its own lists because the splice strategy
// rewires the nodes it is handed.
public sealed partial class MergeInBetweenLinkedListsTests
{
    public static TheoryData<MergeBetweenExample> Examples =>
        new()
        {
            // LeetCode example 1.
            {
                new MergeBetweenExample(
                    List1: [0, 1, 2, 3, 4, 5], A: 3, B: 4,
                    List2: [1000000, 1000001, 1000002],
                    Expected: [0, 1, 2, 1000000, 1000001, 1000002, 5])
            },

            // LeetCode example 2.
            {
                new MergeBetweenExample(
                    List1: [0, 1, 2, 3, 4, 5, 6], A: 2, B: 5,
                    List2: [1000000, 1000001, 1000002, 1000003, 1000004],
                    Expected: [0, 1, 1000000, 1000001, 1000002, 1000003, 1000004, 6])
            },

            // Smallest window the constraints allow: a == b, one node removed.
            {
                new MergeBetweenExample(
                    List1: [0, 1, 2, 3, 4], A: 1, B: 1, List2: [9, 8], Expected: [0, 9, 8, 2, 3, 4])
            },

            // Widest window the constraints allow: everything but the first and last node.
            {
                new MergeBetweenExample(
                    List1: [0, 1, 2, 3, 4], A: 1, B: 3, List2: [7], Expected: [0, 7, 4])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeInBetweenByArrayRebuild_LeetCodeExamples_SplicesList2InPlaceOfRemovedRange(
        MergeBetweenExample example)
    {
        var merged = MergeInBetweenLinkedListsSolution.MergeInBetweenByArrayRebuild(
            BuildList(example.List1), example.A, example.B, BuildList(example.List2));

        Assert.Equal(example.Expected, ToArray(merged));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeInBetweenByPointerSplice_LeetCodeExamples_SplicesList2InPlaceOfRemovedRange(
        MergeBetweenExample example)
    {
        var merged = MergeInBetweenLinkedListsSolution.MergeInBetweenByPointerSplice(
            BuildList(example.List1), example.A, example.B, BuildList(example.List2));

        Assert.Equal(example.Expected, ToArray(merged));
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        // The dummy is the splice point the loop hangs the first real node on; taking
        // its Next drops the dummy from the list that is handed back.
        return dummy.Next
            ?? throw new InvalidOperationException(
                "every example's list1 holds at least one node, so the loop above always links the dummy to a real head");
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

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas. The two
    // bounds are named because a and b are the endpoints of the range removed from
    // list1, and nothing in a bare `3, 4` says which end is which.
    public readonly record struct MergeBetweenExample(
        int[] List1, int A, int B, int[] List2, int[] Expected);
}
