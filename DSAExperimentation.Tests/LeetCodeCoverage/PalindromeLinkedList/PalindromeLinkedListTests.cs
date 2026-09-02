using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.PalindromeLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromeLinkedList;

// Harness only. The single strategy is PalindromeLinkedListSolution's - this file
// builds LeetCode's published examples as linked lists and checks the result.
public sealed class PalindromeLinkedListTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 2, 2, 1], true },
            { [1, 2], false },
            { [1], true },
            { [1, 2, 3, 2, 1], true },
            { [1, 2, 3, 3, 2, 1], true },
            { [1, 2, 1, 3], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByStackReversal_LeetCodeExamples_ReturnsExpected(int[] values, bool expected) =>
        Assert.Equal(expected, PalindromeLinkedListSolution.IsPalindromeByStackReversal(BuildList(values)));

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
}
