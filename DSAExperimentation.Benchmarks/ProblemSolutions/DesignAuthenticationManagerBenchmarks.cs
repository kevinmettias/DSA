using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Authentication Manager (LC 1797): a naive List<(string TokenId, int
// Expiry)> linear-scan manager (the "no hashing at all" baseline a first-pass
// implementation reaches for - DesignHashMapBenchmarks precedent) vs. this repo's
// HashMap<TKey,TValue>. Both are populated with the same Length tokens, then
// renewed with Length probes split evenly between existing and unknown token ids,
// so a linear scan's O(n) cost per lookup is fully exercised on every probe
// (existing-but-unrenewable ids never short-circuit the scan early).
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
    public int LinearScanList()
    {
        var entries = BuildLinearScanEntries();
        RenewLinearScanEntries(entries);
        return CountUnexpired(entries.Select(entry => entry.Expiry));
    }

    private List<(string TokenId, int Expiry)> BuildLinearScanEntries()
    {
        var entries = new List<(string TokenId, int Expiry)>(Length);

        for (var i = 0; i < _tokenIds.Length; i++)
        {
            entries.Add((_tokenIds[i], i + TimeToLive));
        }

        return entries;
    }

    private void RenewLinearScanEntries(List<(string TokenId, int Expiry)> entries)
    {
        foreach (var probe in _renewProbeIds)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].TokenId != probe)
                {
                    continue;
                }

                if (entries[i].Expiry > Length)
                {
                    entries[i] = (probe, Length + TimeToLive);
                }

                break;
            }
        }
    }

    [Benchmark]
    public int RepoHashMap()
    {
        var expiryByToken = BuildHashMapEntries();
        RenewHashMapEntries(expiryByToken);
        return CountUnexpired(expiryByToken.Values);
    }

    private HashMap<string, int> BuildHashMapEntries()
    {
        var expiryByToken = new HashMap<string, int>();

        for (var i = 0; i < _tokenIds.Length; i++)
        {
            expiryByToken.Set(_tokenIds[i], i + TimeToLive);
        }

        return expiryByToken;
    }

    private void RenewHashMapEntries(HashMap<string, int> expiryByToken)
    {
        foreach (var probe in _renewProbeIds)
        {
            if (expiryByToken.TryGetValue(probe, out var expiry) && expiry > Length)
            {
                expiryByToken.Set(probe, Length + TimeToLive);
            }
        }
    }

    private int CountUnexpired(IEnumerable<int> expiries)
    {
        var unexpired = 0;

        foreach (var expiry in expiries)
        {
            if (expiry > Length)
            {
                unexpired++;
            }
        }

        return unexpired;
    }
}
