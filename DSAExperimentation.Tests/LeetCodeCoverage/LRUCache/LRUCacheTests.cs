using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LRUCache;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LRUCache;

// Harness only. Both strategies are LRUCacheSolution's - this file replays
// LeetCode's published call sequence against each ICache<int,int> instance,
// so a failure still names the strategy that broke even though the "input"
// here is a sequence of get/put calls rather than a single argument tuple -
// the same shape DesignTaskManagerTests already uses for its own instance-API
// problem. LRUCacheOp.Apply is pure dispatch plus LeetCode's own -1-on-miss
// convention (ICache<TKey,TValue>.TryGetValue leaves `value` undefined on a
// miss, per its own doc comment) - no eviction logic of its own.
public sealed partial class LRUCacheTests
{
    public static TheoryData<int, LRUCacheOp[], int?[]> Examples =>
        new()
        {
            {
                2,
                [
                    LRUCacheOp.Put(1, 1),
                    LRUCacheOp.Put(2, 2),
                    LRUCacheOp.Get(1),
                    LRUCacheOp.Put(3, 3),
                    LRUCacheOp.Get(2),
                    LRUCacheOp.Put(4, 4),
                    LRUCacheOp.Get(1),
                    LRUCacheOp.Get(3),
                    LRUCacheOp.Get(4),
                ],
                [null, null, 1, null, -1, null, -1, 3, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLruCachePrimitive_LeetCodeExample_EvictsLeastRecentlyUsedKey(
        int capacity, LRUCacheOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(LRUCacheSolution.CreateByLruCachePrimitive(capacity), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDictionaryLinkedList_LeetCodeExample_EvictsLeastRecentlyUsedKey(
        int capacity, LRUCacheOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(LRUCacheSolution.CreateByDictionaryLinkedList(capacity), operations));

    private static int?[] RunScript(ICache<int, int> cache, LRUCacheOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(cache))];

    // One call in an LRUCache script: which method to invoke and with what
    // arguments. Pure dispatch, built via the named factories below so a script
    // (like Examples above) reads like the LeetCode call sequence it replays.
    // Nested here rather than left at file scope so the file declares exactly
    // one type.
    public readonly record struct LRUCacheOp(LRUCacheOp.OpKind kind, int key, int value)
    {
        public static LRUCacheOp Get(int key) => new(OpKind.Get, key, 0);

        public static LRUCacheOp Put(int key, int value) => new(OpKind.Put, key, value);

        // null for put, the returned value for get (LeetCode's own -1-on-miss
        // convention, since ICache<TKey,TValue>.TryGetValue's out value is only
        // meaningful when it returns true) - so a script runner can assert
        // against one expected value per operation uniformly. Internal, not
        // public: only this same assembly's test method ever calls Apply.
        internal int? Apply(ICache<int, int> cache)
        {
            if (kind == OpKind.Put)
            {
                cache.Set(key, value);
                return null;
            }

            return cache.TryGetValue(key, out var cachedValue) ? cachedValue : -1;
        }

        public enum OpKind
        {
            Get,
            Put,
        }
    }
}
