using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.LinkedListRandomNode;

// LeetCode 382. Linked List Random Node: Solution(head) stores a linked list once;
// getRandom() returns one of its values with equal probability, called many times
// against the same instance. Both strategies take the Random explicitly rather
// than owning one internally, so a harness controls seeding and call count instead
// of the strategy hiding its own state.
//
// GetRandomByReservoirSampling is the textbook baseline this composition has to
// justify itself against: O(1) extra space, but an O(n) walk on every single call.
// GetRandomByDynamicArrayCache is the composed answer - walk the list once into
// this repo's own DynamicArray<int>, then GetRandom is a single O(1) indexed read,
// the same value -> DynamicArray<int> "cache once, GetRandom in O(1)" shape
// InsertDeleteGetRandomO1/InsertDeleteGetRandomO1DuplicatesAllowed's own
// GetRandom() already commits to.
internal static class LinkedListRandomNodeSolution
{
    // Classic single-pass reservoir sampling of size 1: the result starts as the
    // 1st element, so the next candidate considered is the 2nd, and each later
    // element replaces the running result with probability 1/rank.
    public static int GetRandomByReservoirSampling(SinglyLinkedListNode<int> head, Random random)
    {
        var result = head.Value;
        var rank = 2;

        for (var node = head.Next; node is not null; node = node.Next)
        {
            if (random.Next(rank) == 0)
            {
                result = node.Value;
            }

            rank++;
        }

        return result;
    }

    public static int GetRandomByDynamicArrayCache(SinglyLinkedListNode<int> head, Random random) =>
        GetRandomByDynamicArrayCache(CacheValues(head), random);

    public static int GetRandomByDynamicArrayCache(DynamicArray<int> cache, Random random) =>
        cache.Get(random.Next(cache.Count));

    // The hoisted prepared-input step: walk the list once into a DynamicArray<int>
    // so a benchmark can charge that O(n) conversion to construction and then
    // measure only the O(1) indexed reads, without ever materializing a BCL
    // collection the LeetCode-shaped overload above could also bind to.
    public static DynamicArray<int> CacheValues(SinglyLinkedListNode<int> head)
    {
        var values = new DynamicArray<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }
}
