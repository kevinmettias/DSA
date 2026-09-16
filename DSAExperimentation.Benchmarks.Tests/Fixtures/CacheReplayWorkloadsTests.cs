using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Cache;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CacheReplayWorkloads (ARCHITECTURE 17.7). Both cache siblings replay the
// same script, so what the fixture owes a test is the script's own shape: Capacity puts that fill
// the cache exactly, then a get/put pair per capacity round drawn from a key range twice as wide,
// so every round mixes a hit on a recently touched key with a miss on an evicted or never-inserted
// one. A recording cache stands in for either sibling, because the script's shape must not depend
// on which eviction policy replays it.
public sealed partial class CacheReplayWorkloadsTests
{
    private const int Capacity = 8;
    private const int Seed = 146; // LC problem number
    private const int InitialPutCount = Capacity;
    private const int RoundCount = Capacity;
    private const int StepsPerRound = 2;
    private const int WidenedKeyRangeDivisor = 2;

    [Fact]
    public void Build_Capacity_ReturnsTheInitialPutsThenOneGetPutPairPerRound() =>
        Assert.Equal(
            InitialPutCount + (RoundCount * StepsPerRound),
            CacheReplayWorkloads.Build(Capacity, Seed).Count);

    [Fact]
    public void Build_InitialPuts_FillTheCacheExactly()
    {
        var script = CacheReplayWorkloads.Build(Capacity, Seed);
        var cache = new RecordingCache();

        for (var step = 0; step < InitialPutCount; step++)
        {
            Assert.Null(script[step](cache));
        }

        Assert.Equal(Capacity, cache.Count);
    }

    [Fact]
    public void Build_LaterRounds_AlternateGetsThatAnswerWithPutsThatDoNot()
    {
        var script = CacheReplayWorkloads.Build(Capacity, Seed);
        var cache = new RecordingCache();
        ReplayInitialPuts(script, cache);

        for (var step = InitialPutCount; step < script.Count; step++)
        {
            var isGetStep = (step - InitialPutCount) % StepsPerRound == 0;

            Assert.Equal(isGetStep, script[step](cache) is not null);
        }
    }

    [Fact]
    public void Build_LaterRounds_ReachKeysBeyondTheOnesTheInitialPutsFilled()
    {
        var script = CacheReplayWorkloads.Build(Capacity, Seed);
        var cache = new RecordingCache();
        ReplayInitialPuts(script, cache);

        for (var step = InitialPutCount; step < script.Count; step++)
        {
            script[step](cache);
        }

        Assert.Contains(cache.TouchedKeys, key => key >= Capacity);
        Assert.All(
            cache.TouchedKeys,
            key => Assert.InRange(key, 0, (Capacity * WidenedKeyRangeDivisor) - 1));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameScript() =>
        Assert.Equal(
            AnswerSum(CacheReplayWorkloads.Build(Capacity, Seed)),
            AnswerSum(CacheReplayWorkloads.Build(Capacity, Seed)));

    private static void ReplayInitialPuts(
        IReadOnlyList<Func<ICache<int, int>, int?>> script, ICache<int, int> cache)
    {
        for (var step = 0; step < InitialPutCount; step++)
        {
            script[step](cache);
        }
    }

    // Sums every answered get rather than discarding it - the same shape the benchmark's own
    // replay uses, so a script that changed its answers would show up here too.
    private static long AnswerSum(IReadOnlyList<Func<ICache<int, int>, int?>> script)
    {
        var cache = new RecordingCache();
        var sum = 0L;

        foreach (var step in script)
        {
            sum += step(cache) ?? 0;
        }

        return sum;
    }

    // Records which keys the script touches, so a test can check the key range the replay reaches
    // over without depending on either sibling's eviction policy.
    private sealed class RecordingCache : ICache<int, int>
    {
        private readonly Dictionary<int, int> _entries = new();
        private readonly HashSet<int> _touchedKeys = new();

        public int Count => _entries.Count;

        public IReadOnlyCollection<int> TouchedKeys => _touchedKeys;

        public bool TryGetValue(int key, out int value)
        {
            _touchedKeys.Add(key);

            return _entries.TryGetValue(key, out value);
        }

        public void Set(int key, int value)
        {
            _touchedKeys.Add(key);
            _entries[key] = value;
        }
    }
}
