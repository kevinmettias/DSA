using DSAExperimentation.LeetCode.LRUCache;

namespace DSAExperimentation.LeetCode.Tests.LRUCache;

// The seam between LRUCacheSolution's two caches. CreateByLruCachePrimitive hands
// back a live DataStructures.LruCache<int, int> - this repo's own HashMap beside a
// DoublyLinkedList - while CreateByDictionaryLinkedList writes the same recency
// rule against a BCL Dictionary and LinkedList.
//
// A cache is nothing but its lifecycle: capacity is fixed at construction and every
// Set past it evicts whichever key the recency list currently calls oldest. Neither
// arm can be checked from a single call, because the state that decides what a call
// returns was built by the calls before it. So each test replays one complete script
// against both and compares the whole transcript, which is the only place a
// recency-list that stopped matching its index, or a capacity bound that drifted by
// one, becomes visible.
public sealed partial class LruCacheEvictionSeamTests
{
    private const int Capacity = 3;

    [Fact]
    public void CacheScript_EvictsLeastRecentlyUsedPastCapacity_MatchesDictionaryAndList()
    {
        CacheOperation[] script =
        [
            CacheOperation.Put(1, 1),
            CacheOperation.Put(2, 2),
            CacheOperation.Get(1),
            CacheOperation.Put(3, 3),
            CacheOperation.Get(2),
            CacheOperation.Put(4, 4),
            CacheOperation.Get(1),
            CacheOperation.Get(3),
            CacheOperation.Get(4),
        ];

        Assert.Equal(
            ["null", "null", "1", "null", "2", "null", "-1", "3", "4"],
            Replay(Capacity, script, usePrimitive: true));
    }

    // A read is what makes a key recent, so the same put sequence with the get moved
    // must evict a different key - the half of the rule a put-only script cannot see.
    [Fact]
    public void Get_ChangesWhichKeyIsEvictedNext_MatchesDictionaryAndList()
    {
        CacheOperation[] unreadScript =
        [
            CacheOperation.Put(1, 1),
            CacheOperation.Put(2, 2),
            CacheOperation.Put(3, 3),
            CacheOperation.Get(1),
            CacheOperation.Get(3),
        ];
        CacheOperation[] readScript =
        [
            CacheOperation.Put(1, 1),
            CacheOperation.Put(2, 2),
            CacheOperation.Get(1),
            CacheOperation.Put(3, 3),
            CacheOperation.Get(1),
            CacheOperation.Get(3),
        ];

        Assert.Equal(
            ["null", "null", "null", "-1", "3"],
            Replay(capacity: 2, unreadScript, usePrimitive: true));
        Assert.Equal(
            ["null", "null", "1", "null", "1", "3"],
            Replay(capacity: 2, readScript, usePrimitive: true));
        AssertSameTranscript(capacity: 2, unreadScript);
        AssertSameTranscript(capacity: 2, readScript);
    }

    // Capacity 1: every put of a new key evicts the only other key, so the recency
    // list is never longer than one and the eviction branch runs on every call.
    [Fact]
    public void CacheScript_CapacityOne_MatchesDictionaryAndList()
    {
        CacheOperation[] script =
        [
            CacheOperation.Put(1, 10),
            CacheOperation.Put(2, 20),
            CacheOperation.Get(1),
            CacheOperation.Get(2),
            CacheOperation.Put(3, 30),
            CacheOperation.Get(2),
            CacheOperation.Get(3),
        ];

        Assert.Equal(
            ["null", "null", "-1", "20", "null", "-1", "30"],
            Replay(capacity: 1, script, usePrimitive: true));
        AssertSameTranscript(capacity: 1, script);
    }

    // Overwriting an existing key must refresh its value and its recency without
    // growing the cache, so a script that re-puts the same keys forever must never
    // evict anything.
    [Fact]
    public void Put_ExistingKey_RefreshesWithoutEvicting_MatchesDictionaryAndList()
    {
        CacheOperation[] script =
        [
            CacheOperation.Put(1, 1),
            CacheOperation.Put(2, 2),
            CacheOperation.Put(1, 100),
            CacheOperation.Put(2, 200),
            CacheOperation.Put(1, 101),
            CacheOperation.Get(1),
            CacheOperation.Get(2),
        ];

        Assert.Equal(
            ["null", "null", "null", "null", "null", "101", "200"],
            Replay(Capacity, script, usePrimitive: true));
        AssertSameTranscript(Capacity, script);
    }

    [Fact]
    public void Get_MissingKey_ReportsAFailureOnBothCaches()
    {
        CacheOperation[] script = [CacheOperation.Get(7), CacheOperation.Put(7, 70), CacheOperation.Get(7)];

        Assert.Equal(["-1", "null", "70"], Replay(Capacity, script, usePrimitive: true));
        AssertSameTranscript(Capacity, script);
    }

    private static void AssertSameTranscript(int capacity, CacheOperation[] script)
        => Assert.Equal(Replay(capacity, script, usePrimitive: true), Replay(capacity, script, usePrimitive: false));

    private static List<string> Replay(int capacity, CacheOperation[] script, bool usePrimitive)
    {
        var cache = usePrimitive
            ? LRUCacheSolution.CreateByLruCachePrimitive(capacity)
            : LRUCacheSolution.CreateByDictionaryLinkedList(capacity);
        var transcript = new List<string>(script.Length);

        foreach (var operation in script)
        {
            transcript.Add(operation.IsPut
                ? "null"
                : cache.TryGetValue(operation.Key, out var value) ? value.ToString() : "-1");

            if (operation.IsPut)
            {
                cache.Set(operation.Key, operation.Value);
            }
        }

        return transcript;
    }

    private readonly record struct CacheOperation(bool IsPut, int Key, int Value)
    {
        public static CacheOperation Put(int key, int value) => new(IsPut: true, key, value);

        public static CacheOperation Get(int key) => new(IsPut: false, key, Value: 0);
    }
}
