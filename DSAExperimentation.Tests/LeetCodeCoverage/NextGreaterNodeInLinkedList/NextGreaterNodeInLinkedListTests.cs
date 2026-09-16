using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.NextGreaterNodeInLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterNodeInLinkedList;

// Harness only. Both strategies are NextGreaterNodeInLinkedListSolution's - this
// file states LeetCode's examples once as the list's values plus the expected
// per-node answer, and asserts each strategy against them.
public sealed partial class NextGreaterNodeInLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [2, 1, 5], [5, 5, 0] },
            { [2, 7, 4, 3, 5], [7, 0, 5, 5, 0] },
            { [1, 7, 5, 1, 9, 2, 5, 1], [7, 9, 9, 9, 0, 5, 0, 0] },
            { [9, 7, 5, 3], [0, 0, 0, 0] },
            { [1, 2, 3, 4], [2, 3, 4, 0] },
            { [4, 4, 4, 4], [0, 0, 0, 0] },
            { [1], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextLargerNodesByBruteForceScan_LeetCodeExamples_ReturnsNextGreaterPerNode(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            NextGreaterNodeInLinkedListSolution.NextLargerNodesByBruteForceScan(BuildList(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextLargerNodesByMonotonicStackSweep_LeetCodeExamples_ReturnsNextGreaterPerNode(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            NextGreaterNodeInLinkedListSolution.NextLargerNodesByMonotonicStackSweep(BuildList(values)));

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
