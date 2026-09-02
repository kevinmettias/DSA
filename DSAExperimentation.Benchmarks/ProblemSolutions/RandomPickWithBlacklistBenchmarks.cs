using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Random Pick with Blacklist (LC 710): naive rejection sampling (redraw from [0, N)
// until landing outside the blacklist - the natural first solution attempt) vs. this
// repo's Set<int> + HashMap<int,int> remap, which turns every Pick() into a single
// O(1)-expected draw regardless of blacklist size. Only WhitelistSize (a small constant)
// numbers are ever left un-blacklisted while N grows, so a uniform draw's success
// probability is ~WhitelistSize/N - rejection sampling's expected retries per pick grow
// linearly with N, while the remap solution's Pick() stays O(1) after its one-time O(B)
// constructor-equivalent setup (paid once per benchmark invocation here, outside the
// per-call loop, mirroring how a real Solution only builds the remap once).
[MemoryDiagnoser]
public class RandomPickWithBlacklistBenchmarks
{
    private const int WhitelistSize = 10;
    private const int PickCalls = 200;

    [Params(2_000, 100_000)]
    public int N;

    private int[] _blacklist = null!;

    [GlobalSetup]
    public void Setup()
    {
        var blacklistSize = N - WhitelistSize;
        _blacklist = Enumerable.Range(0, blacklistSize).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long RejectionSampling()
    {
        var blacklisted = new HashSet<int>(_blacklist);
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            int candidate;
            do
            {
                candidate = random.Next(N);
            } while (blacklisted.Contains(candidate));

            total += candidate;
        }

        return total;
    }

    [Benchmark]
    public long SetHashMapRemap()
    {
        var whitelistBound = N - _blacklist.Length;
        var blacklistedSet = BuildBlacklistedSet(_blacklist);
        var remap = BuildRemap(whitelistBound, _blacklist, blacklistedSet);
        return PickWithRemap(whitelistBound, remap);
    }

    private static Set<int> BuildBlacklistedSet(int[] blacklist)
    {
        var blacklistedSet = new Set<int>();
        foreach (var value in blacklist)
        {
            blacklistedSet.TryAdd(value);
        }

        return blacklistedSet;
    }

    private static HashMap<int, int> BuildRemap(int whitelistBound, int[] blacklist, Set<int> blacklistedSet)
    {
        var remapBuilder = new BlacklistRemapBuilder(whitelistBound, blacklistedSet);
        foreach (var value in blacklist)
        {
            remapBuilder.MapIfBlacklistedBelowBound(value);
        }

        return remapBuilder.Remap;
    }

    private static long PickWithRemap(int whitelistBound, HashMap<int, int> remap)
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var candidate = random.Next(whitelistBound);
            total += remap.TryGetValue(candidate, out var mapped) ? mapped : candidate;
        }

        return total;
    }

    private sealed class BlacklistRemapBuilder(int whitelistBound, Set<int> blacklistedSet)
    {
        private readonly int _whitelistBound = whitelistBound;
        private int _nextWhitelisted = whitelistBound;

        public HashMap<int, int> Remap { get; } = new();

        public void MapIfBlacklistedBelowBound(int value)
        {
            if (value >= _whitelistBound)
            {
                return;
            }

            while (blacklistedSet.Has(_nextWhitelisted))
            {
                _nextWhitelisted++;
            }

            Remap.Set(value, _nextWhitelisted);
            _nextWhitelisted++;
        }
    }
}
