using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseLinkedListII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseLinkedListII;

// Harness only. Both strategies are ReverseLinkedListIISolution's - this file
// builds LeetCode's published examples as linked lists and checks the resulting
// list's values.
public sealed partial class ReverseLinkedListIITests
{
    public static TheoryData<int[], int, int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, 4, [1, 4, 3, 2, 5] },
            { [5], 1, 1, [5] },
            { [1, 2, 3, 4, 5], 1, 5, [5, 4, 3, 2, 1] },
            { [1, 2, 3, 4, 5], 1, 3, [3, 2, 1, 4, 5] },
            { [1, 2, 3, 4, 5], 3, 5, [1, 2, 5, 4, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseBetweenByArrayRebuild_LeetCodeExamples_ReversesClosedRange(
        int[] values, int left, int right, int[] expected)
    {
        var reversed = ReverseLinkedListIISolution.ReverseBetweenByArrayRebuild(BuildList(values), left, right);
        var actual = ToArray(reversed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseBetweenByHeadInsertion_LeetCodeExamples_ReversesClosedRange(
        int[] values, int left, int right, int[] expected)
    {
        var reversed = ReverseLinkedListIISolution.ReverseBetweenByHeadInsertion(BuildList(values), left, right);
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
