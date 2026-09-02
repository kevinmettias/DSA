using System.Numerics;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.AddTwoNumbers;

// LeetCode 2. Add Two Numbers: each operand is a singly linked list storing its
// digits least-significant digit first, so the sum is built the same way, one
// SinglyLinkedListNode<int> at a time.
//
// The two strategies differ only in how they get the digit sum: walk both lists
// once with a running carry, or convert each list to a single big number, add,
// and convert back.
internal static class AddTwoNumbersSolution
{
    private const int DecimalBase = 10;

    // The composed solution: walk both lists exactly once with a running carry,
    // splicing new nodes onto a dummy head as it goes - O(n) with a single pass.
    public static SinglyLinkedListNode<int>? AddByDigitwiseListWalk(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var carry = 0;

        while (first is not null || second is not null || carry != 0)
        {
            var digitSum = carry + (first?.Value ?? 0) + (second?.Value ?? 0);
            carry = digitSum / DecimalBase;

            tail.Next = new SinglyLinkedListNode<int>(digitSum % DecimalBase);
            tail = tail.Next;

            first = first?.Next;
            second = second?.Next;
        }

        return dummy.Next;
    }

    // The textbook answer many first reach for: convert each digit list to a
    // BigInteger and back. Each `* 10` grows the running total's limb count by
    // one, so accumulating an n-digit number this way costs O(n^2) overall versus
    // DigitwiseListWalk's O(n). Deliberately written without this repo's
    // primitives beyond the input/output list shape itself.
    public static SinglyLinkedListNode<int>? AddByBigIntegerConvertAndBack(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var sum = ToBigInteger(first) + ToBigInteger(second);
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        do
        {
            tail.Next = new SinglyLinkedListNode<int>((int)(sum % DecimalBase));
            tail = tail.Next;
            sum /= DecimalBase;
        }
        while (sum > 0);

        return dummy.Next;
    }

    private static BigInteger ToBigInteger(SinglyLinkedListNode<int>? head)
    {
        BigInteger value = 0;
        BigInteger placeValue = 1;

        for (var node = head; node is not null; node = node.Next)
        {
            value += node.Value * placeValue;
            placeValue *= DecimalBase;
        }

        return value;
    }
}
