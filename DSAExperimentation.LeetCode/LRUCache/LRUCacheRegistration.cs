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
            .Build();

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
