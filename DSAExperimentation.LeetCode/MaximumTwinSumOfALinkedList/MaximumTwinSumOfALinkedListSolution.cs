using DSAExperimentation.DataStructures.Deque;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MaximumTwinSumOfALinkedList;

// LeetCode 2130. Maximum Twin Sum of a Linked List: in a list of even length n,
// node i and node (n - 1 - i) are twins, and the answer is the largest sum of any
// twin pair.
//
// A singly linked list gives no way to reach node (n - 1 - i) from node i, so both
// strategies buffer the values on one forward walk and then pair them off; they
// differ only in what does the pairing. ArrayIndexTwoPointer materializes a BCL
// List<int> and indexes it from both ends. DequeFrontBackDrain uses this repo's
// own Deque<int> (CircularBuffer-backed, ARCHITECTURE.md Sec.4.1): twins arrive at
// the deque's two ends in lockstep, so draining a front and a back together yields
// each twin pair with no index arithmetic at all.
internal static class MaximumTwinSumOfALinkedListSolution
{
    // Twins pair the first half with the second, so exactly n / 2 pairs exist.
    private const int TwinPairStride = 2;

    // The textbook answer: copy every value into a BCL List<int>, then walk i up
    // from the front while indexing ^(i + 1) back from the end. Deliberately
    // written with nothing from this repo but the node it is handed - it is the arm
    // the deque drain below has to justify itself against.
    public static int PairSumByArrayIndexTwoPointer(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var best = 0;

        for (var i = 0; i < values.Count / TwinPairStride; i++)
        {
            best = Math.Max(best, values[i] + values[^(i + 1)]);
        }

        return best;
    }

    // One forward walk pushes every value onto the back of a Deque<int>; popping a
    // front and a back together then hands back exactly one twin pair per
    // iteration, and the loop ends on its own when the deque runs dry.
    public static int PairSumByDequeFrontBackDrain(SinglyLinkedListNode<int>? head)
    {
        var values = new Deque<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.PushBack(node.Value);
        }

        var best = 0;

        while (values.TryPopFront(out var front) && values.TryPopBack(out var back))
        {
            best = Math.Max(best, front + back);
        }

        return best;
    }
}
