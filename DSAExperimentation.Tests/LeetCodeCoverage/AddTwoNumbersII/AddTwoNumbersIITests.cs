using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddTwoNumbersII;

// LeetCode 445. Add Two Numbers II: digits are stored most-significant-first here
// (the reverse of AddTwoNumbersTests' LC 2 order), so digit-wise addition has to
// start from each list's tail, not its head. This repo's own Stack<T> - the same
// LIFO primitive ReverseIntegerTests uses - reverses each list's digit order without
// ever mutating the input lists: push every value while walking .Next once, then pop
// both stacks in lockstep with a running carry, prepending each result digit onto
// the front of the output list as it's produced.
public sealed partial class AddTwoNumbersIITests
{
    [Fact]
    public void Add_ClassicExample_ReturnsDigitwiseSumInOrder()
    {
        var first = BuildList([7, 2, 4, 3]);
        var second = BuildList([5, 6, 4]);

        var sum = AddNumbers(first, second);

        Assert.Equal([7, 8, 0, 7], ToArray(sum));
    }

    [Fact]
    public void Add_CarryCascadesPastBothLists_AddsLeadingDigit()
    {
        var first = BuildList([9, 9, 9]);
        var second = BuildList([1]);

        var sum = AddNumbers(first, second);

        Assert.Equal([1, 0, 0, 0], ToArray(sum));
    }

    [Fact]
    public void Add_DifferentLengths_HandlesShorterSecondOperand()
    {
        var first = BuildList([8, 4, 6]);
        var second = BuildList([5]);

        var sum = AddNumbers(first, second);

        Assert.Equal([8, 5, 1], ToArray(sum));
    }

    private static SinglyLinkedListNode<int>? AddNumbers(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var firstDigits = new NumberStack();
        for (var node = first; node is not null; node = node.Next)
        {
            firstDigits.Push(node.Value);
        }

        var secondDigits = new NumberStack();
        for (var node = second; node is not null; node = node.Next)
        {
            secondDigits.Push(node.Value);
        }

        SinglyLinkedListNode<int>? head = null;
        var carry = 0;

        while (firstDigits.Count > 0 || secondDigits.Count > 0 || carry != 0)
        {
            var a = firstDigits.TryPop(out var firstDigit) ? firstDigit : 0;
            var b = secondDigits.TryPop(out var secondDigit) ? secondDigit : 0;
            var digitSum = carry + a + b;
            carry = digitSum / 10;

            head = new SinglyLinkedListNode<int>(digitSum % 10) { Next = head };
        }

        return head;
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
