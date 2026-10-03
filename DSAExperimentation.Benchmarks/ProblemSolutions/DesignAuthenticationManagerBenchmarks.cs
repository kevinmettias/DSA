using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DesignAuthenticationManager;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAuthenticationManagerSolution's, the same
// classes DesignAuthenticationManagerSolutionTests proves correct - the naive
// List<(string, int)> linear-scan manager (the "no hashing at all" baseline a
// first-pass implementation reaches for, DesignHashMapBenchmarks precedent)
// against this repo's HashMap<TKey,TValue>. Both are populated with the same
// Length tokens, then renewed with Length probes split evenly between existing and
// unknown token ids, so a linear scan's O(n) cost per lookup is fully exercised on
// every probe. [GlobalSetup] materializes the token ids so building them is charged
// to setup rather than to the replay; they are LowercaseNames, since LC 1797 spells a
// token id in at most five lowercase letters. LC 1797 also allows 2000 calls in all:
// Length generates, Length renews and the one count make 999 the largest Length.
public class DesignAuthenticationManagerBenchmarks
{
    // The clock advances one per call, so a time-to-live as long as LC 1797's whole
    // 2000-call script keeps every token alive to the end.
    private const int TimeToLive = 2_000;

    private const int AlternatingModulus = 2;

    private string[] _tokenIds = [];

    private string[] _renewProbeIds = [];

    [Params(200, 999)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _tokenIds = Enumerable.Range(0, Length).Select(LowercaseNames.Of).ToArray();

        // Half hits (existing tokens), half misses (unknown ids) - same hit/miss
        // split as DesignHashMapBenchmarks, so both a successful renew's
        // lookup-then-overwrite and a failed renew's full failed-lookup path run on
        // every probe.
        _renewProbeIds = Enumerable.Range(0, Length).Select(ProbeIdAt).ToArray();
    }

    private static bool IsEvenIndex(int index) => index % AlternatingModulus == 0;

    // An even probe names the token generated at that index; an odd one names an id
    // past every generated token's, which no Generate call ever saw.
    private string ProbeIdAt(int index)
    {
        if (IsEvenIndex(index))
        {
            return _tokenIds[index];
        }

        return LowercaseNames.Of(Length + index);
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new DesignAuthenticationManagerSolution.AuthenticationManagerByLinearScanList(TimeToLive));

    [Benchmark]
    public int RepoHashMap() => Replay(new DesignAuthenticationManagerSolution.AuthenticationManagerByHashMap(TimeToLive));

    // LC 1797's currentTime starts at 1 and strictly increases, so every call here
    // comes one tick after the last. Nothing expires before the count, so a hit
    // finds a live token and renews it, and the count is every token generated.
    private int Replay(DesignAuthenticationManagerSolution.IAuthenticationManager manager)
    {
        var currentTime = 0;

        foreach (var tokenId in _tokenIds)
        {
            currentTime++;
            manager.Generate(tokenId, currentTime);
        }

        foreach (var probe in _renewProbeIds)
        {
            currentTime++;
            manager.Renew(probe, currentTime);
        }

        currentTime++;

        return manager.CountUnexpiredTokens(currentTime);
    }
}
