using DSAExperimentation.DataStructures.Cache;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the two cache siblings - LC 146 (LRU) and LC 460
// (LFU) - whose replay scripts are the same: Capacity initial puts fill the cache
// exactly, then Capacity rounds of get/put run over a key range twice as wide, so
// some calls hit a recently touched key and some miss an evicted or never-inserted
// one. Building the script is charged to [GlobalSetup]; only the replay is measured.
internal static class CacheReplayWorkloads
{
    public static List<Func<ICache<int, int>, int?>> Build(int capacity, int seed)
    {
        var random = new Random(seed);
        var script = new List<Func<ICache<int, int>, int?>>();

        for (var key = 0; key < capacity; key++)
        {
            var value = random.Next(0, capacity);

            // Named separately because a for-loop variable is one variable shared by every
            // iteration: a closure capturing `key` directly would leave all capacity puts
            // writing the loop's exit value, so the cache would hold one entry instead of
            // the full capacity this script is supposed to fill it to.
            var filledKey = key;

            script.Add(cache =>
            {
                cache.Set(filledKey, value);
                return null;
            });
        }

        var roundKeyUpperBound = capacity * 2;

        for (var round = 0; round < capacity; round++)
        {
            AppendGetThenPut(script, capacity, roundKeyUpperBound, random);
        }

        return script;
    }

    // The get/put pair one measured round replays: a get of a key drawn from the wider
    // range the later rounds reach over, then a put of a fresh value under another
    // such key.
    private static void AppendGetThenPut(
        List<Func<ICache<int, int>, int?>> script, int capacity, int roundKeyUpperBound, Random random)
    {
        var getKey = random.Next(0, roundKeyUpperBound);
        script.Add(cache => cache.TryGetValue(getKey, out var value) ? value : -1);

        var putKey = random.Next(0, roundKeyUpperBound);
        var putValue = random.Next(0, capacity);
        script.Add(cache =>
        {
            cache.Set(putKey, putValue);
            return null;
        });
    }
}
