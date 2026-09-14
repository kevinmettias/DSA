using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.InsertGreatestCommonDivisorsInLinkedList;

// LeetCode 2807. Insert Greatest Common Divisors in Linked List: between every
// pair of adjacent nodes, insert a node holding the greatest common divisor of
// the two values, and return the head of the modified list.
//
// Both strategies walk the list once and reduce each adjacent pair with the same
// Euclidean step - two-integer GCD has no repo container or algorithm primitive
// to compose over, which is the reasoning CheckIfItIsAGoodArraySolution and
// CheckIfPointIsReachableSolution already record for keeping it a private helper.
// What differs is where the answer is assembled: a separate value buffer that the
// whole sequence is rebuilt from, or the original nodes with one fresh node
// spliced between each pair, so nothing but the inserted nodes is allocated.
internal static class InsertGreatestCommonDivisorsInLinkedListSolution
{
    // The textbook answer: copy the values out into a List<int>, interleave the
    // gcds into a second List<int>, then thread a brand new list through them.
    // Deliberately written with nothing but BCL buffers - it is the arm the
    // in-place splice below has to justify itself against, and it allocates a
    // node for every element rather than only for the ones being inserted.
    public static SinglyLinkedListNode<int>? InsertGreatestCommonDivisorsByValueRebuild(
        SinglyLinkedListNode<int>? head)
    {
        var original = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            original.Add(node.Value);
        }

        return BuildList(Interleaved(original));
    }

    // nums[0], gcd(nums[0], nums[1]), nums[1], ... - the gcd goes in front of
    // every element except the first, which has no predecessor to pair with.
    private static List<int> Interleaved(List<int> original)
    {
        var gaps = Math.Max(original.Count - 1, 0);
        var interleaved = new List<int>(original.Count + gaps);

        for (var i = 0; i < original.Count; i++)
        {
            if (i > 0)
            {
                var gcd = Gcd(original[i - 1], original[i]);
                interleaved.Add(gcd);
            }

            interleaved.Add(original[i]);
        }

        return interleaved;
    }

    private static SinglyLinkedListNode<int>? BuildList(List<int> values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    // Pointer rewiring over the mutable SinglyLinkedListNode<int> the repo already
    // has (SwapNodesInPairs' precedent): allocate one node per gap, stitch it
    // between the pair, and resume from the node that used to be Next - so the
    // walk is a single pass and the only allocations are the inserted gcd nodes.
    public static SinglyLinkedListNode<int>? InsertGreatestCommonDivisorsByNodeSplice(
        SinglyLinkedListNode<int>? head)
    {
        var current = head;

        while (current?.Next is not null)
        {
            var gcdNode = new SinglyLinkedListNode<int>(Gcd(current.Value, current.Next.Value))
            {
                Next = current.Next,
            };

            current.Next = gcdNode;
            current = gcdNode.Next;
        }

        return head;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
