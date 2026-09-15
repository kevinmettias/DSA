using System.Numerics;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.AddTwoNumbersII;

// LeetCode 445. Add Two Numbers II: digits are stored most-significant-first here
// (the reverse of AddTwoNumbers' LC 2 order), so digit-wise addition has to start
// from each list's tail, not its head.
//
// AddByBigInteger is the naive baseline: convert each list to a BigInteger by
// walking it head-to-tail (`value = value * 10 + digit`, an O(n) multiply-by-10 on
// a growing n-digit BigInteger at every step, O(n^2) overall - the same limb-growth
// cost AddTwoNumbers' BigIntegerConvertAndBack baseline pays for LC 2's
// least-significant-first variant), add, then unpack the sum's decimal digits back
// into a list - deliberately written without this repo's primitives beyond the
// input/output list itself. AddByTwoStacks instead pushes each list's digits onto
// this repo's own Stack<T> while walking .Next once, then pops both stacks in
// lockstep with a running carry, prepending each result digit onto the front of the
// output list as it's produced - O(n), with neither input list ever reversed or
// mutated.
internal static class AddTwoNumbersIISolution
{
    private const int DecimalBase = 10;

    public static SinglyLinkedListNode<int>? AddByBigInteger(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var sum = ToBigInteger(first) + ToBigInteger(second);

        SinglyLinkedListNode<int>? head = null;

        do
        {
            head = new SinglyLinkedListNode<int>((int)(sum % DecimalBase)) { Next = head };
            sum /= DecimalBase;
        }
        while (sum > 0);

        return head;
    }

    private static BigInteger ToBigInteger(SinglyLinkedListNode<int>? node)
    {
        BigInteger value = 0;

        for (; node is not null; node = node.Next)
        {
            value = (value * DecimalBase) + node.Value;
        }

        return value;
    }

    public static SinglyLinkedListNode<int>? AddByTwoStacks(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var firstDigits = new NumberStack();
        PushDigits(firstDigits, first);

        var secondDigits = new NumberStack();
        PushDigits(secondDigits, second);

        SinglyLinkedListNode<int>? head = null;
        var carry = 0;

        while (HasColumnLeft(firstDigits, secondDigits, carry))
        {
            var a = firstDigits.TryPop(out var firstDigit) ? firstDigit : 0;
            var b = secondDigits.TryPop(out var secondDigit) ? secondDigit : 0;
            var digitSum = carry + a + b;
            carry = digitSum / DecimalBase;

            head = new SinglyLinkedListNode<int>(digitSum % DecimalBase) { Next = head };
        }

        return head;
    }

    private static void PushDigits(NumberStack stack, SinglyLinkedListNode<int>? node)
    {
        for (; node is not null; node = node.Next)
        {
            stack.Push(node.Value);
        }
    }

    // Another column of the sum is owed while either stack still holds a digit or
    // the previous column left a carry behind.
    private static bool HasColumnLeft(NumberStack firstDigits, NumberStack secondDigits, int carry)
        => firstDigits.Count > 0 || secondDigits.Count > 0 || carry != 0;
}
