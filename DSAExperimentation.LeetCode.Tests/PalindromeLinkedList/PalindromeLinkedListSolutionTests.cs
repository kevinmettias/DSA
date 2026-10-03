using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PalindromeLinkedList;

namespace DSAExperimentation.LeetCode.Tests.PalindromeLinkedList;

// Harness only. Two strategies live in PalindromeLinkedListSolution - the stack
// reversal and the fast/slow in-place reversal - so this file builds LeetCode's
// published examples as linked lists and checks each strategy against them. Each
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
        var head = LeetCodeWireFormat.ToLinkedList(example.Values);
        var isPalindrome = PalindromeLinkedListSolution.IsPalindromeByStackReversal(head);

        Assert.Equal(example.Expected, isPalindrome);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByFastSlowReversal_LeetCodeExamples_ReturnsExpected(ListCase example)
    {
        var head = LeetCodeWireFormat.ToLinkedList(example.Values);
        var isPalindrome = PalindromeLinkedListSolution.IsPalindromeByFastSlowReversal(head);

        Assert.Equal(example.Expected, isPalindrome);
    }

    // One LeetCode example: the values of the list, in order, and whether they read the
    // same forwards and backwards. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct ListCase(int[] Values, bool Expected);
}
