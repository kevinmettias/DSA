using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DoubleANumberRepresentedAsALinkedList;

// LeetCode 2816. Double a Number Represented as a Linked List: digits are stored
// most-significant-first, the same order AddTwoNumbersIITests' operands use, so
// doubling has to start from the list's tail, not its head. This repo's own
// Stack<T> - the same LIFO primitive AddTwoNumbersIITests reverses each operand
// with - pushes every digit while walking .Next once, then pops them
// least-significant-first with a running carry, prepending each doubled digit onto
// the front of the result as it's produced. No reversal of the input list, no
// mutation.
public sealed partial class DoubleANumberRepresentedAsALinkedListTests
{
    [Fact]
    public void Double_LeetCodeExample1_ReturnsDoubledValueWithNoCarryIntoNewDigit()
    {
        var head = BuildList([1, 8, 9]);

        var doubled = DoubleNumber(head);

        Assert.Equal([3, 7, 8], ToArray(doubled));
    }

    [Fact]
    public void Double_LeetCodeExample2_CarryCascadesIntoNewLeadingDigit()
    {
        var head = BuildList([9, 9, 9]);

        var doubled = DoubleNumber(head);

        Assert.Equal([1, 9, 9, 8], ToArray(doubled));
    }

    [Fact]
    public void Double_SingleZeroDigit_ReturnsZero()
    {
        var head = BuildList([0]);

        var doubled = DoubleNumber(head);

        Assert.Equal([0], ToArray(doubled));
    }

    private static SinglyLinkedListNode<int>? DoubleNumber(SinglyLinkedListNode<int>? head)
    {
        var digits = new NumberStack();
        for (var node = head; node is not null; node = node.Next)
        {
            digits.Push(node.Value);
        }

        return DoubleDigitStack(digits);
    }

    private static SinglyLinkedListNode<int>? DoubleDigitStack(NumberStack digits)
    {
        SinglyLinkedListNode<int>? result = null;
        var carry = 0;

        while (digits.Count > 0 || carry != 0)
        {
            var digit = digits.TryPop(out var value) ? value : 0;
            var doubled = (digit * 2) + carry;
            carry = doubled / 10;

            result = new SinglyLinkedListNode<int>(doubled % 10) { Next = result };
        }

        return result;
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
