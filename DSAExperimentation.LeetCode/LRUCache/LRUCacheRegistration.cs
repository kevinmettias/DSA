using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.LRUCache;

// Shape under test: a design problem with a CONSTRUCTOR ARGUMENT and more than
// one implementation behind it. Min Stack proves a script can drive a stateful
// object; this proves the script can also carry the constructor's own input, and
// that two independent implementations can be held to the same script - which is
// the whole point of the strategy axis for a design problem.
internal sealed class LRUCacheRegistration : ILeetCodeProblemRegistration
{
    private const int MissingValue = -1;
    private const int ScriptSeed = 146;

    // The two operation names the script and the replay must agree on. They are
    // named once here because the switch in `Apply` below is the only place that
    // decides what a name means, and a `Case` that spelled one differently from
    // the switch would be rejected at run time as an unknown operation rather
    // than at compile time as a typo.
    private const string Put = "put";
    private const string Get = "get";

    // What choosing a cache implementation means: take the capacity a script supplies
    // and hand back the cache to replay it against. An ICache<int, int> parameter would
    // name one already-built cache; the decision here is which implementation gets
    // built, so that is what the type says.
    private interface ICacheFactory
    {
        ICache<int, int> Create(int capacity);
    }

    // LRUCacheSolution exposes its arms as static factory methods, so each needs a
    // one-method adapter to satisfy the interface. Both are stateless, so one instance
    // each is shared: the strategies registered below run inside the benchmark
    // harness's timed region, and picking an arm must not allocate there.
    private static readonly ICacheFactory LruCachePrimitive = new LruCachePrimitiveFactory();
    private static readonly ICacheFactory DictionaryLinkedList = new DictionaryLinkedListFactory();

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int Capacity, IReadOnlyList<LeetCodeOperation> Script), List<int?>>("lru-cache")
            .Strategy("LruCachePrimitive", input => Replay(LruCachePrimitive, input))
            .Strategy("DictionaryLinkedList", input => Replay(DictionaryLinkedList, input))
            .MatchingAnswersWith(LeetCodeAnswers.IsSequenceEqual)
            .Case(
                "example-1",
                (2,
                [
                    LeetCodeOperation.Of(Put, 1, 1),
                    LeetCodeOperation.Of(Put, 2, 2),
                    LeetCodeOperation.Of(Get, 1),
                    LeetCodeOperation.Of(Put, 3, 3),
                    LeetCodeOperation.Of(Get, 2),
                    LeetCodeOperation.Of(Put, 4, 4),
                    LeetCodeOperation.Of(Get, 1),
                    LeetCodeOperation.Of(Get, 3),
                    LeetCodeOperation.Of(Get, 4),
                ]),
                [null, null, 1, null, MissingValue, null, MissingValue, 3, 4])
            .Case(
                "reading-a-key-makes-it-most-recent",
                (2,
                [
                    LeetCodeOperation.Of(Put, 1, 1),
                    LeetCodeOperation.Of(Put, 2, 2),
                    LeetCodeOperation.Of(Get, 1),
                    LeetCodeOperation.Of(Put, 3, 3),
                    LeetCodeOperation.Of(Get, 1),
                ]),
                [null, null, 1, null, 1])
            .Case(
                "overwriting-a-key-does-not-grow-the-cache",
                (1,
                [
                    LeetCodeOperation.Of(Put, 1, 1),
                    LeetCodeOperation.Of(Put, 1, 9),
                    LeetCodeOperation.Of(Get, 1),
                ]),
                [null, null, 9])

            // The two capacities the retired per-problem benchmark swept. The
            // script fills the cache exactly, then runs `capacity` rounds of
            // get/put over twice the key range, so some calls hit recently touched
            // keys and some miss evicted or never-inserted ones - a cache that is
            // never made to evict measures nothing an ordinary dictionary wouldn't.
            .Workload("mixed-hits-and-misses-200", BuildScriptedWorkload(200))
            .Workload("mixed-hits-and-misses-2000", BuildScriptedWorkload(2_000))
            .Build();

    private static List<int?> Replay(
        ICacheFactory createCache, (int Capacity, IReadOnlyList<LeetCodeOperation> Script) input)
    {
        var cache = createCache.Create(input.Capacity);
        var results = new List<int?>(input.Script.Count);

        foreach (var operation in input.Script)
        {
            var applied = Apply(cache, operation);
            results.Add(applied);
        }

        return results;
    }

    private static int? Apply(ICache<int, int> cache, LeetCodeOperation operation)
    {
        switch (operation.Name)
        {
            case Put:
                cache.Set(operation.Arguments[0], operation.Arguments[1]);
                return null;
            case Get:
                return cache.TryGetValue(operation.Arguments[0], out var value) ? value : MissingValue;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation), operation.Name, "LRU Cache has no such operation.");
        }
    }

    private static (int Capacity, IReadOnlyList<LeetCodeOperation> Script) BuildScriptedWorkload(int capacity)
    {
        var random = new Random(ScriptSeed);
        var script = new List<LeetCodeOperation>();

        for (var key = 0; key < capacity; key++)
        {
            var value = random.Next(0, capacity);
            var insertion = LeetCodeOperation.Of(Put, key, value);
            script.Add(insertion);
        }

        for (var round = 0; round < capacity; round++)
        {
            AppendMixedRound(script, random, capacity);
        }

        return (capacity, script);
    }

    // One round of the mixed phase: a read somewhere in twice the key range, then a write that
    // may hit a recently read key, a stale one, or a key never inserted - which is what makes
    // the phase measure eviction rather than insertion.
    private static void AppendMixedRound(List<LeetCodeOperation> script, Random random, int capacity)
    {
        var keyUpperBound = capacity * 2;

        var keyToRead = random.Next(0, keyUpperBound);
        var read = LeetCodeOperation.Of(Get, keyToRead);
        script.Add(read);

        var keyToWrite = random.Next(0, keyUpperBound);
        var valueToWrite = random.Next(0, capacity);
        var write = LeetCodeOperation.Of(Put, keyToWrite, valueToWrite);
        script.Add(write);
    }

    private sealed class LruCachePrimitiveFactory : ICacheFactory
    {
        public ICache<int, int> Create(int capacity) => LRUCacheSolution.CreateByLruCachePrimitive(capacity);
    }

    private sealed class DictionaryLinkedListFactory : ICacheFactory
    {
        public ICache<int, int> Create(int capacity) => LRUCacheSolution.CreateByDictionaryLinkedList(capacity);
    }
}
