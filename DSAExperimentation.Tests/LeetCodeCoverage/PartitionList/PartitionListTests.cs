using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.PartitionList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionList;

// Harness only. Both strategies are PartitionListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed class PartitionListTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 4, 3, 2, 5, 2], 3, [1, 2, 2, 4, 3, 5] },
            { [2, 1], 2, [1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByArrayRebuild_LeetCodeExamples_PartitionsList(
        int[] values, int x, int[] expected)
    {
        var partitioned = PartitionListSolution.PartitionByArrayRebuild(BuildList(values), x);
        var actual = ToArray(partitioned);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByPointerSplice_LeetCodeExamples_PartitionsList(
        int[] values, int x, int[] expected)
    {
        var partitioned = PartitionListSolution.PartitionByPointerSplice(BuildList(values), x);
        var actual = ToArray(partitioned);

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
