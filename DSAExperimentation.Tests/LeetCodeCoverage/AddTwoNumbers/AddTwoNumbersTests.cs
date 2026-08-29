using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddTwoNumbers;

// LeetCode 2. Add Two Numbers: a single digit-wise walk over this repo's own
// SinglyLinkedListNode<int>.Next, the same dummy-head list-building shape
// MergeTwoSortedListsTests uses. Each list already stores its least-significant
// digit first, so the carry only ever flows left-to-right as the walk advances -
// no reversal needed.
public sealed partial class AddTwoNumbersTests
{
    [Fact]
    public void Add_ClassicExample_ReturnsDigitwiseSumInReverseOrder()
    {
        var first = BuildList([2, 4, 3]);
        var second = BuildList([5, 6, 4]);

        var sum = AddNumbers(first, second);

        Assert.Equal([7, 0, 8], ToArray(sum));
    }

    [Fact]
    public void Add_CarryCascadesPastBothLists_AddsLeadingDigit()
    {
        var first = BuildList([9, 9, 9]);
        var second = BuildList([1]);

        var sum = AddNumbers(first, second);

        Assert.Equal([0, 0, 0, 1], ToArray(sum));
    }

    [Fact]
    public void Add_DifferentLengths_PadsShorterListWithZeros()
    {
        var first = BuildList([2, 4, 9]);
        var second = BuildList([5, 6]);

        var sum = AddNumbers(first, second);

        Assert.Equal([7, 0, 0, 1], ToArray(sum));
    }

    private static SinglyLinkedListNode<int>? AddNumbers(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var carry = 0;

        while (first is not null || second is not null || carry != 0)
        {
            var digitSum = carry + (first?.Value ?? 0) + (second?.Value ?? 0);
            carry = digitSum / 10;

            tail.Next = new SinglyLinkedListNode<int>(digitSum % 10);
            tail = tail.Next;

            first = first?.Next;
            second = second?.Next;
        }

        return dummy.Next;
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
