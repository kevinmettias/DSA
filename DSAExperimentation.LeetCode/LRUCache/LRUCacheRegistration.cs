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

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int Capacity, IReadOnlyList<LeetCodeOperation> Script), List<int?>>("lru-cache")
            .Strategy("LruCachePrimitive", input => Replay(LRUCacheSolution.CreateByLruCachePrimitive, input))
            .Strategy("DictionaryLinkedList", input => Replay(LRUCacheSolution.CreateByDictionaryLinkedList, input))
            .MatchingAnswersWith(LeetCodeAnswers.SequenceEqual)
            .Case(
                "example-1",
                (2,
                [
                    LeetCodeOperation.Of("put", 1, 1),
                    LeetCodeOperation.Of("put", 2, 2),
                    LeetCodeOperation.Of("get", 1),
                    LeetCodeOperation.Of("put", 3, 3),
                    LeetCodeOperation.Of("get", 2),
                    LeetCodeOperation.Of("put", 4, 4),
                    LeetCodeOperation.Of("get", 1),
                    LeetCodeOperation.Of("get", 3),
                    LeetCodeOperation.Of("get", 4),
                ]),
                [null, null, 1, null, MissingValue, null, MissingValue, 3, 4])
            .Case(
                "reading-a-key-makes-it-most-recent",
                (2,
                [
                    LeetCodeOperation.Of("put", 1, 1),
                    LeetCodeOperation.Of("put", 2, 2),
                    LeetCodeOperation.Of("get", 1),
                    LeetCodeOperation.Of("put", 3, 3),
                    LeetCodeOperation.Of("get", 1),
                ]),
                [null, null, 1, null, 1])
            .Case(
                "overwriting-a-key-does-not-grow-the-cache",
                (1,
                [
                    LeetCodeOperation.Of("put", 1, 1),
                    LeetCodeOperation.Of("put", 1, 9),
                    LeetCodeOperation.Of("get", 1),
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

    private static (int Capacity, IReadOnlyList<LeetCodeOperation> Script) BuildScriptedWorkload(int capacity)
    {
        var random = new Random(ScriptSeed);
        var script = new List<LeetCodeOperation>();

        for (var key = 0; key < capacity; key++)
        {
            script.Add(LeetCodeOperation.Of("put", key, random.Next(0, capacity)));
        }

        var keyUpperBound = capacity * 2;

        for (var round = 0; round < capacity; round++)
        {
            script.Add(LeetCodeOperation.Of("get", random.Next(0, keyUpperBound)));
            script.Add(LeetCodeOperation.Of("put", random.Next(0, keyUpperBound), random.Next(0, capacity)));
        }

        return (capacity, script);
    }

    private static List<int?> Replay(
        Func<int, ICache<int, int>> createCache, (int Capacity, IReadOnlyList<LeetCodeOperation> Script) input)
    {
        var cache = createCache(input.Capacity);
        var results = new List<int?>(input.Script.Count);

        foreach (var operation in input.Script)
        {
            results.Add(Apply(cache, operation));
        }

        return results;
    }

    private static int? Apply(ICache<int, int> cache, LeetCodeOperation operation)
    {
        switch (operation.Name)
        {
            case "put":
                cache.Set(operation.Arguments[0], operation.Arguments[1]);
                return null;
            case "get":
                return cache.TryGetValue(operation.Arguments[0], out var value) ? value : MissingValue;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation), operation.Name, "LRU Cache has no such operation.");
        }
    }
}
