using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveNthNodeFromEndOfList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveNthNodeFromEndOfList;

// Harness only. Both strategies are RemoveNthNodeFromEndOfListSolution's - this
// file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed class RemoveNthNodeFromEndOfListTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [1, 2, 3, 5] },
            { [1], 1, [] },
            { [1, 2], 2, [2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByArrayRebuild_LeetCodeExamples_RemovesExpectedNode(
        int[] values, int positionsFromEnd, int[] expected)
    {
        var removed = RemoveNthNodeFromEndOfListSolution.RemoveByArrayRebuild(BuildList(values), positionsFromEnd);
        var actual = ToArray(removed);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByTwoRunner_LeetCodeExamples_RemovesExpectedNode(
        int[] values, int positionsFromEnd, int[] expected)
    {
        var removed = RemoveNthNodeFromEndOfListSolution.RemoveByTwoRunner(BuildList(values), positionsFromEnd);
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
