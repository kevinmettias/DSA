using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddTwoNumbers;

// Harness only. Both strategies are AddTwoNumbersSolution's - this file pins them
// to LeetCode's published examples, stated once as digit arrays in the same
// least-significant-digit-first order LC 2's own lists use.
public sealed class AddTwoNumbersTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [2, 4, 3], [5, 6, 4], [7, 0, 8] },
            { [9, 9, 9], [1], [0, 0, 0, 1] },
            { [2, 4, 9], [5, 6], [7, 0, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByDigitwiseListWalk_LeetCodeExamples_ReturnsDigitwiseSumInReverseOrder(
        int[] first, int[] second, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(AddTwoNumbersSolution.AddByDigitwiseListWalk(BuildList(first), BuildList(second))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBigIntegerConvertAndBack_LeetCodeExamples_ReturnsDigitwiseSumInReverseOrder(
        int[] first, int[] second, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(AddTwoNumbersSolution.AddByBigIntegerConvertAndBack(BuildList(first), BuildList(second))));

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
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
