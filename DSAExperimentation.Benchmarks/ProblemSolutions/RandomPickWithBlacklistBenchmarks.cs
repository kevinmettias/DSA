using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RandomPickWithBlacklist;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomPickWithBlacklistSolution's, the same factories
// RandomPickWithBlacklistTests proves correct. Random Pick with Blacklist (LC 710):
// naive rejection sampling (redraw from [0, N) until landing outside the blacklist -
// the natural first solution attempt) vs. this repo's Set<int> + HashMap<int,int>
// remap, which turns every Pick() into a single O(1)-expected draw regardless of
// blacklist size. Only WhitelistSize (a small constant) numbers are ever left
// un-blacklisted while N grows, so a uniform draw's success probability is
// ~WhitelistSize/N - rejection sampling's expected retries per pick grow linearly with
// N, while the remap solution's Pick() stays O(1) after its one-time O(B)
// constructor-equivalent setup. [GlobalSetup] only builds the blacklist array itself
// (workload sizing); each benchmark method still pays its own strategy's
// constructor-equivalent setup cost once, then PickCalls picks - mirroring how a real
// Solution instance only builds its representation once per program lifetime.
[MemoryDiagnoser]
public class RandomPickWithBlacklistBenchmarks
{
    private const int WhitelistSize = 10;
    private const int PickCalls = 200;
    private const int Seed = 1;

    private int[] _blacklist = [];

    [Params(2_000, 100_000)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var blacklistSize = N - WhitelistSize;
        _blacklist = Enumerable.Range(0, blacklistSize).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long RejectionSampling()
    {
        var randomPick = RandomPickWithBlacklistSolution.CreateByRejectionSampling(N, _blacklist, Seed);

        return Replay(randomPick);
    }

    [Benchmark]
    public long SetHashMapRemap()
    {
        var randomPick = RandomPickWithBlacklistSolution.CreateBySetHashMapRemap(N, _blacklist, Seed);

        return Replay(randomPick);
    }

    private static long Replay(RandomPickWithBlacklistSolution.IRandomPick randomPick)
    {
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            total += randomPick.Pick();
        }

        return total;
    }
}
