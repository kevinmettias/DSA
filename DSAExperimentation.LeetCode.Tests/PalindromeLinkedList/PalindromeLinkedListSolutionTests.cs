using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.PalindromeLinkedList;

namespace DSAExperimentation.LeetCode.Tests.PalindromeLinkedList;

// Harness only. Two strategies live in PalindromeLinkedListSolution - the stack
// reversal and the fast/slow in-place reversal - so this file builds LeetCode's
// published examples as linked lists, checks both, and checks they agree. Each
// example is built as a fresh list per arm.
public sealed partial class PalindromeLinkedListSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByFastSlowReversal_LeetCodeExamples_ReturnsExpected(ListCase example)
    {
        var head = BuildList(example.Values);
        var isPalindrome = PalindromeLinkedListSolution.IsPalindromeByFastSlowReversal(head);

        Assert.Equal(example.Expected, isPalindrome);
    }

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they reach the same verdict on every example - not merely that
    // each agrees with the expectation beside it. Each arm gets its own list.
    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindrome_AgreeOnEveryExample(ListCase example)
    {
        var byStackReversal = PalindromeLinkedListSolution.IsPalindromeByStackReversal(BuildList(example.Values));
        var byFastSlowReversal = PalindromeLinkedListSolution.IsPalindromeByFastSlowReversal(BuildList(example.Values));

        Assert.Equal(byStackReversal, byFastSlowReversal);
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
