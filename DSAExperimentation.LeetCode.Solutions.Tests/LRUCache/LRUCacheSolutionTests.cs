using DSAExperimentation.Mechanisms.Cache;
using DSAExperimentation.LeetCode.Solutions.LRUCache;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LRUCache;

// Harness only. Both strategies are LRUCacheSolution's - this file replays
// call scripts the catalog does not hold against each ICache<int,int> instance;
// LeetCode's published example runs in CatalogCaseTests. LRUCacheOp.Apply is
// pure dispatch through LRUCacheCalls, which states LeetCode's -1 for a missing
// key - no eviction logic of its own.
public sealed partial class LRUCacheSolutionTests
{
    public static TheoryData<int, LRUCacheOp[], int?[]> Scripts =>
        new()
        {
            // Reading a key makes it the most recent, so the next eviction takes the
            // other one.
            {
                2,
                [
                    LRUCacheOp.Put(1, 1),
                    LRUCacheOp.Put(2, 2),
                    LRUCacheOp.Get(1),
                    LRUCacheOp.Put(3, 3),
                    LRUCacheOp.Get(1),
                ],
                [null, null, 1, null, 1]
            },

            // Overwriting a key replaces its value without growing the cache, so a
            // capacity-1 cache still holds it.
            {
                1,
                [
                    LRUCacheOp.Put(1, 1),
                    LRUCacheOp.Put(1, 9),
                    LRUCacheOp.Get(1),
                ],
                [null, null, 9]
            },
        };

    [Theory]
    [MemberData(nameof(Scripts))]
    public void CreateByLruCachePrimitive_Script_EvictsLeastRecentlyUsedKey(
        int capacity, LRUCacheOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(LRUCacheSolution.CreateByLruCachePrimitive(capacity), operations));

    [Theory]
    [MemberData(nameof(Scripts))]
    public void CreateByDictionaryLinkedList_Script_EvictsLeastRecentlyUsedKey(
        int capacity, LRUCacheOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(LRUCacheSolution.CreateByDictionaryLinkedList(capacity), operations));

    private static int?[] RunScript(ICache<int, int> cache, LRUCacheOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(cache))];

    // One call in an LRUCache script: which method to invoke and with what
    // arguments. Pure dispatch, built via the named factories below so a script
    // (like Scripts above) reads like the LeetCode call sequence it replays.
    // Nested here rather than left at file scope so the file declares exactly
    // one type.
    public readonly record struct LRUCacheOp(LRUCacheOp.OpKind kind, int key, int value)
    {
        public static LRUCacheOp Get(int key) => new(OpKind.Get, key, 0);

        public static LRUCacheOp Put(int key, int value) => new(OpKind.Put, key, value);

        // null for put, the returned value for get - so a script runner can
        // assert against one expected value per operation uniformly. Internal,
        // not public: only this same assembly's test method ever calls Apply.
        internal int? Apply(ICache<int, int> cache)
        {
            if (kind == OpKind.Put)
            {
                LRUCacheCalls.Put(cache, key, value);
                return null;
            }

            return LRUCacheCalls.Get(cache, key);
        }

        public enum OpKind
        {
            Get,
            Put,
        }
    }
}
