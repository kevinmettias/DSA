using DSAExperimentation.LeetCode.DoubleANumberRepresentedAsALinkedList;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DoubleANumberRepresentedAsALinkedList;

// Harness only. Both strategies are DoubleANumberRepresentedAsALinkedListSolution's
// - including the BigInteger baseline, which the benchmark used to own privately
// and nothing asserted. SinglyLinkedListNode<int> is internal, so it cannot appear
// in a public TheoryData<...> member (CS0053); the examples state the digits and
// LeetCodeWireFormat translates both ends.
public sealed class DoubleANumberRepresentedAsALinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LC example 1: 189 doubles to 378, no new leading digit.
            { [1, 8, 9], [3, 7, 8] },

            // LC example 2: 999 doubles to 1998, so the carry cascades the whole
            // way and grows the list by one node.
            { [9, 9, 9], [1, 9, 9, 8] },

            // The shortest list LC allows, and the only one whose leading digit
            // may be 0.
            { [0], [0] },

            // A single digit that still carries out: 5 doubles to 10.
            { [5], [1, 0] },

            // Carry out of the last two positions only, so a strategy that
            // propagated it one place too far would be caught: 455 -> 910.
            { [4, 5, 5], [9, 1, 0] },

            // Nothing carries anywhere: 100 -> 200.
            { [1, 0, 0], [2, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DoubleNumberByBigInteger_LeetCodeExamples_ReturnsDigitsOfTwiceTheNumber(
        int[] digits, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByBigInteger(
                    LeetCodeWireFormat.ToLinkedList(digits))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DoubleNumberByDigitStack_LeetCodeExamples_ReturnsDigitsOfTwiceTheNumber(
        int[] digits, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByDigitStack(
                    LeetCodeWireFormat.ToLinkedList(digits))));
}
