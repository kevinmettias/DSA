using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbersII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddTwoNumbersII;

// Harness only. Both strategies are AddTwoNumbersIISolution's - this file just
// pins them to LeetCode's published examples, converting each row's operand/result
// arrays to/from the repo's own SinglyLinkedListNode<int>.
public sealed partial class AddTwoNumbersIITests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [7, 2, 4, 3], [5, 6, 4], [7, 8, 0, 7] },
            { [9, 9, 9], [1], [1, 0, 0, 0] },
            { [8, 4, 6], [5], [8, 5, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBigInteger_LeetCodeExamples_ReturnsDigitwiseSumInOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersIISolution.AddByBigInteger(BuildList(first), BuildList(second));
        var actual = ToArray(sum);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByTwoStacks_LeetCodeExamples_ReturnsDigitwiseSumInOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersIISolution.AddByTwoStacks(BuildList(first), BuildList(second));
        var actual = ToArray(sum);

        Assert.Equal(expected, actual);
    }

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
