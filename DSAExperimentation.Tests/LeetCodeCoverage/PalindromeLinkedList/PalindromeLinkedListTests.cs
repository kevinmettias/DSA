using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.PalindromeLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromeLinkedList;

// Harness only. The single strategy is PalindromeLinkedListSolution's - this file
// builds LeetCode's published examples as linked lists and checks the result.
public sealed class PalindromeLinkedListTests
{
    public static TheoryData<ListCase> Examples =>
        new()
        {
            { new ListCase(Values: [1, 2, 2, 1], Expected: true) },
            { new ListCase(Values: [1, 2], Expected: false) },
            { new ListCase(Values: [1], Expected: true) },
            { new ListCase(Values: [1, 2, 3, 2, 1], Expected: true) },
            { new ListCase(Values: [1, 2, 3, 3, 2, 1], Expected: true) },
            { new ListCase(Values: [1, 2, 1, 3], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByStackReversal_LeetCodeExamples_ReturnsExpected(ListCase example)
    {
        var head = BuildList(example.Values);
        var isPalindrome = PalindromeLinkedListSolution.IsPalindromeByStackReversal(head);

        Assert.Equal(example.Expected, isPalindrome);
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

    // One LeetCode example: the values of the list, in order, and whether they read the
    // same forwards and backwards. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct ListCase(int[] Values, bool Expected);
}
