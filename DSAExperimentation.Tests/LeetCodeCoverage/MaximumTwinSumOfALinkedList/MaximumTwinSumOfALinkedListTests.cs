using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MaximumTwinSumOfALinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTwinSumOfALinkedList;

// Harness only. Both strategies are MaximumTwinSumOfALinkedListSolution's -
// including the index-pairing baseline, which the benchmark used to own privately
// and nothing asserted. SinglyLinkedListNode<int> is internal, so it cannot appear
// in a public TheoryData<...> member (CS0053); the examples state the node values
// and each theory builds the chain, the same shape
// FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsTests uses.
public sealed class MaximumTwinSumOfALinkedListTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LC example 1: twin sums are 5 + 1 and 4 + 2.
            { [5, 4, 2, 1], 6 },

            // LC example 2: twin sums are 4 + 3 and 2 + 2.
            { [4, 2, 2, 3], 7 },

            // LC example 3: the shortest list LC allows is a single twin pair.
            { [1, 100_000], 100_001 },

            // Every pair sums the same, so the maximum is not the first pair by
            // accident.
            { [1, 2, 3, 4, 5, 6], 7 },

            // The largest pair is the innermost one, reached last by both walks.
            { [1, 1, 9, 8, 1, 1], 17 },

            // The largest pair is the outermost one, reached first by both walks.
            { [9, 1, 1, 1, 1, 8], 17 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PairSumByArrayIndexTwoPointer_LeetCodeExamples_ReturnsMaximumTwinSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MaximumTwinSumOfALinkedListSolution.PairSumByArrayIndexTwoPointer(BuildList(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PairSumByDequeFrontBackDrain_LeetCodeExamples_ReturnsMaximumTwinSum(
        int[] values, int expected) =>
        Assert.Equal(
            expected,
            MaximumTwinSumOfALinkedListSolution.PairSumByDequeFrontBackDrain(BuildList(values)));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        foreach (var value in values[1..])
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return head;
    }
}
