using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ConvertBinaryNumberInALinkedListToInteger;

// LeetCode 1290. Convert Binary Number in a Linked List to Integer: the list holds
// a binary number most-significant bit first; return its decimal value.
//
// Both strategies answer with an int, LeetCode's own answer shape (the list is at
// most 30 nodes, so the value fits). Neither needs an algorithm primitive beyond
// the node representation itself - the same "just the representation" shape
// MiddleOfTheLinkedListSolution walks - so the comparison is purely how the bits
// are folded: one pass with no buffer, or two passes through an intermediate list.
internal static class ConvertBinaryNumberInALinkedListToIntegerSolution
{
    // A single left-to-right walk over SinglyLinkedListNode<int>.Next, folding
    // value = (value << 1) | bit as it goes. The list is touched once and nothing
    // is allocated.
    public static int GetDecimalValueBySinglePassShift(SinglyLinkedListNode<int>? head)
    {
        var value = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            value = (value << 1) | node.Value;
        }

        return value;
    }

    // The textbook baseline: collect every bit into a buffer, then fold positional
    // weights right-to-left. Deliberately a BCL List<int> and nothing from this
    // repo - it is what you would write without the fold above - and it pays for
    // an intermediate allocation plus a second pass over the bits.
    public static int GetDecimalValueByCollectThenFold(SinglyLinkedListNode<int>? head)
    {
        var bits = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            bits.Add(node.Value);
        }

        var value = 0;
        var weight = 1;

        for (var i = bits.Count - 1; i >= 0; i--)
        {
            value += bits[i] * weight;
            weight <<= 1;
        }

        return value;
    }
}
