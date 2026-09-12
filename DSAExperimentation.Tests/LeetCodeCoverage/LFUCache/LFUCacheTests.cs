using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LFUCache;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LFUCache;

// Harness only. Both strategies are LFUCacheSolution's - this file replays
// LeetCode's published call sequences against each ICache<int,int> instance, so
// a failure still names the strategy that broke even though the "input" here is
// a sequence of get/put calls rather than a single argument tuple - the same
// shape LRUCacheTests already uses for its own instance-API peer problem.
// LFUCacheOp.Apply is pure dispatch plus LeetCode's own -1-on-miss convention
// (ICache<TKey,TValue>.TryGetValue leaves `value` undefined on a miss, per its
// own doc comment) - no eviction logic of its own.
public sealed class LFUCacheTests
{
    public static TheoryData<int, LFUCacheOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's own worked example, including the frequency tie
                // broken by least-recently-used (put(4,4) evicts key 1, not 3).
                2,
                [
                    LFUCacheOp.Put(1, 1),
                    LFUCacheOp.Put(2, 2),
                    LFUCacheOp.Get(1),
                    LFUCacheOp.Put(3, 3),
                    LFUCacheOp.Get(2),
                    LFUCacheOp.Get(3),
                    LFUCacheOp.Put(4, 4),
                    LFUCacheOp.Get(1),
                    LFUCacheOp.Get(3),
                    LFUCacheOp.Get(4),
                ],
                [null, null, 1, null, -1, 3, null, -1, 3, 4]
            },
            {
                // The original coverage entry's own (thinner) example, kept for
                // continuity: a plain frequency-based eviction with no tie.
                2,
                [
                    LFUCacheOp.Put(1, 1),
                    LFUCacheOp.Put(2, 2),
                    LFUCacheOp.Get(1),
                    LFUCacheOp.Put(3, 3),
                    LFUCacheOp.Get(2),
                    LFUCacheOp.Get(3),
                ],
                [null, null, 1, null, -1, 3]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLfuCachePrimitive_LeetCodeExamples_EvictsLeastFrequentlyUsedKey(
        int capacity, LFUCacheOp[] operations, int?[] expected) =>
        RunScript(LFUCacheSolution.CreateByLfuCachePrimitive(capacity), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDictionaryLinearScan_LeetCodeExamples_EvictsLeastFrequentlyUsedKey(
        int capacity, LFUCacheOp[] operations, int?[] expected) =>
        RunScript(LFUCacheSolution.CreateByDictionaryLinearScan(capacity), operations, expected);

    private static void RunScript(ICache<int, int> cache, LFUCacheOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(cache));
        }
    }
}

// One call in an LFUCache script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct LFUCacheOp
{
    private readonly Kind _kind;
    private readonly int _key;
    private readonly int _value;

    private LFUCacheOp(Kind kind, int key, int value)
    {
        _kind = kind;
        _key = key;
        _value = value;
    }

    public static LFUCacheOp Get(int key) => new(Kind.Get, key, 0);

    public static LFUCacheOp Put(int key, int value) => new(Kind.Put, key, value);

    // null for put, the returned value for get (LeetCode's own -1-on-miss
    // convention, since ICache<TKey,TValue>.TryGetValue's out value is only
    // meaningful when it returns true) - so a script runner can assert
    // against one expected value per operation uniformly. Internal, not
    // public: only this same assembly's test method ever calls Apply.
    internal int? Apply(ICache<int, int> cache)
    {
        if (_kind == Kind.Put)
        {
            cache.Set(_key, _value);
            return null;
        }

        return cache.TryGetValue(_key, out var value) ? value : -1;
    }

    private enum Kind
    {
        Get,
        Put,
    }
}
