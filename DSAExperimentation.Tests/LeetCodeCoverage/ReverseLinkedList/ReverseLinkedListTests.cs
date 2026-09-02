using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseLinkedList;

// Harness only. The single strategy is ReverseLinkedListSolution's - this file
// builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed class ReverseLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [5, 4, 3, 2, 1] },
            { [1, 2], [2, 1] },
            { [], [] },
            { [1], [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReverseListByIterativeRewire_LeetCodeExamples_ReversesPointers(int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(ReverseLinkedListSolution.ReverseListByIterativeRewire(BuildList(values))));

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
