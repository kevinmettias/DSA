using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.LinkedListRandomNode;

// LeetCode 382. Linked List Random Node: Solution(head) stores a linked list once;
// getRandom() returns one of its values with equal probability, called many times
// against the same instance. Each strategy's draw function takes the Random
// explicitly rather than owning one internally, so a harness controls seeding and
// call count instead of the strategy hiding its own state; only the object
// LeetCode's constructor makes (below) owns one, as LeetCode's class must.
//
// GetRandomByReservoirSampling is the textbook baseline this composition has to
// justify itself against: O(1) extra space, but an O(n) walk on every single call.
// GetRandomByDynamicArrayCache is the composed answer - walk the list once into
// this repo's own DynamicArray<int>, then GetRandom is a single O(1) indexed read,
// the same value -> DynamicArray<int> "cache once, GetRandom in O(1)" shape
// InsertDeleteGetRandomO1/InsertDeleteGetRandomO1DuplicatesAllowed's own
// GetRandom() already commits to.
//
// LeetCode's own shape is an object: Solution(head) once, then getRandom() per call.
// Each strategy's overload taking only the head is that constructor - it makes an
// IRandomNode whose GetRandom takes one draw by the strategy's own function below,
// from a Random the object owns - so the catalog harness replays LeetCode's call
// scripts on it, while the functions taking a Random stay what the hand-written
// benchmark and the tests drive. getRandom may answer any value in the list, so an
// answer check judges each script (LinkedListRandomNodeAnswerCheck) rather than the
// recorded draws.
[LeetCodeQuestion("linked-list-random-node", LeetCodeAnswerOrder.AsWritten, AnswerCheck = typeof(LinkedListRandomNodeAnswerCheck))]
internal static class LinkedListRandomNodeSolution
{
    // LeetCode's constructor takes no seed, so each object an overload below makes
    // draws from a Random of its own, seeded alike so that every replay of a script
    // draws the same values and a failing draw reproduces.
    private const int DrawSeed = 382;

    // The object LeetCode's constructor makes: it keeps the head and walks the whole
    // list on every getRandom.
    public static IRandomNode GetRandomByReservoirSampling(SinglyLinkedListNode<int> head) => new ReservoirSamplingNode(head);

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

    // The object LeetCode's constructor makes: it walks the list once, into the cache,
    // and answers every getRandom with one indexed read.
    public static IRandomNode GetRandomByDynamicArrayCache(SinglyLinkedListNode<int> head) => new DynamicArrayCacheNode(head);

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

    // What LeetCode's Solution class answers: one value of its list per call, each
    // node equally likely.
    internal interface IRandomNode
    {
        int GetRandom();
    }

    // GetRandomByReservoirSampling, one draw per call against the list it was made with.
    private sealed class ReservoirSamplingNode(SinglyLinkedListNode<int> head) : IRandomNode
    {
        private readonly Random _random = new(DrawSeed);

        public int GetRandom() => GetRandomByReservoirSampling(head, _random);
    }

    // GetRandomByDynamicArrayCache, the list cached once when the object is made.
    private sealed class DynamicArrayCacheNode(SinglyLinkedListNode<int> head) : IRandomNode
    {
        private readonly DynamicArray<int> _cache = CacheValues(head);
        private readonly Random _random = new(DrawSeed);

        public int GetRandom() => GetRandomByDynamicArrayCache(_cache, _random);
    }
}
