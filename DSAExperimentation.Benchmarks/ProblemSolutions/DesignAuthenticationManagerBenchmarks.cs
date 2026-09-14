using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignAuthenticationManager.DesignAuthenticationManagerSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAuthenticationManagerSolution's, the same
// classes DesignAuthenticationManagerTests proves correct - the naive
// List<(string, int)> linear-scan manager (the "no hashing at all" baseline a
// first-pass implementation reaches for, DesignHashMapBenchmarks precedent)
// against this repo's HashMap<TKey,TValue>. Both are populated with the same
// Length tokens, then renewed with Length probes split evenly between existing and
// unknown token ids, so a linear scan's O(n) cost per lookup is fully exercised on
// every probe (existing-but-unrenewable ids never short-circuit the scan early).
// [GlobalSetup] materializes the token ids so string formatting is charged to
// setup rather than to the replay.
[MemoryDiagnoser]
public class DesignAuthenticationManagerBenchmarks
{
    private const int TimeToLive = 100;
    private const int AlternatingModulus = 2;

    [Params(200, 5_000)]
    public int Length;

    private string[] _tokenIds = null!;
    private string[] _renewProbeIds = null!;

    [GlobalSetup]
    public void Setup()
    {
        _tokenIds = Enumerable.Range(0, Length).Select(i => $"token-{i}").ToArray();

        // Half hits (existing tokens), half misses (unknown ids) - same hit/miss
        // split as DesignHashMapBenchmarks, so both a successful renew's
        // lookup-then-overwrite and a failed renew's full failed-lookup path run on
        // every probe.
        _renewProbeIds = Enumerable.Range(0, Length)
            .Select(i => i % AlternatingModulus == 0 ? _tokenIds[i] : $"missing-{i}")
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new AuthenticationManagerByLinearScanList(TimeToLive));

    [Benchmark]
    public int RepoHashMap() => Replay(new AuthenticationManagerByHashMap(TimeToLive));

    // Token i is generated at time i, so it expires at i + TimeToLive; every renew
    // and the final count then happen at time Length, so only the most recently
    // generated tokens are still alive - a hit on an older one pays for the whole
    // lookup and then declines to renew, which is exactly the path that never
    // short-circuits.
    private int Replay(IAuthenticationManager manager)
    {
        for (var i = 0; i < _tokenIds.Length; i++)
        {
            manager.Generate(_tokenIds[i], currentTime: i);
        }

        foreach (var probe in _renewProbeIds)
        {
            manager.Renew(probe, currentTime: Length);
        }

        return manager.CountUnexpiredTokens(currentTime: Length);
    }
}
