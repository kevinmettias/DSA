using System.Numerics;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.DoubleANumberRepresentedAsALinkedList;

// LeetCode 2816. Double a Number Represented as a Linked List: digits are stored
// most-significant-first, the same order AddTwoNumbersII's operands use, so
// doubling has to start from the list's tail, not its head.
//
// DoubleNumberByBigInteger is the naive baseline - convert the whole list to a
// BigInteger by walking it head-to-tail (`value = value * 10 + digit`, an O(n)
// multiply-by-10 on a growing n-digit BigInteger at every step, O(n^2) overall),
// double it, then unpack the product's decimal digits back into a list. It is
// written with nothing but BCL types beyond the answer's own node type, because
// that is what you would reach for without this repo.
// DoubleNumberByDigitStack instead pushes every digit onto this repo's own
// Stack<T> while walking .Next once, then pops them least-significant-first with a
// running carry, prepending each doubled digit onto the front of the result as it
// is produced - O(n), with the input list never reversed or mutated.
internal static class DoubleANumberRepresentedAsALinkedListSolution
{
    private const int DecimalBase = 10;

    // The digit-doubling factor LC 2816 asks for, spelled out so the carry walk
    // below reads as "double this digit" rather than as an anonymous 2.
    private const int Factor = 2;

    public static SinglyLinkedListNode<int>? DoubleNumberByBigInteger(SinglyLinkedListNode<int>? head)
    {
        var doubled = ToBigInteger(head) * Factor;

        SinglyLinkedListNode<int>? result = null;

        // do/while, not while: a list of a single 0 doubles to 0, which still has
        // one digit to report.
        do
        {
            result = new SinglyLinkedListNode<int>((int)(doubled % DecimalBase)) { Next = result };
            doubled /= DecimalBase;
        }
        while (doubled > 0);

        return result;
    }

    private static BigInteger ToBigInteger(SinglyLinkedListNode<int>? head)
    {
        BigInteger value = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            value = (value * DecimalBase) + node.Value;
        }

        return value;
    }

    public static SinglyLinkedListNode<int>? DoubleNumberByDigitStack(SinglyLinkedListNode<int>? head)
    {
        var digits = new NumberStack();

        for (var node = head; node is not null; node = node.Next)
        {
            digits.Push(node.Value);
        }

        SinglyLinkedListNode<int>? result = null;
        var carry = 0;

        while (digits.Count > 0 || carry != 0)
        {
            var digit = digits.TryPop(out var value) ? value : 0;
            var doubled = (digit * Factor) + carry;
            carry = doubled / DecimalBase;

            result = new SinglyLinkedListNode<int>(doubled % DecimalBase) { Next = result };
        }

        return result;
    }
}
