using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Encode and Decode TinyURL (LC 535): Decode is the operation worth benchmarking -
// looking an already-encoded short URL back up. LinearScanDecode is the naive
// baseline, an O(n) scan over every (short, long) pair recorded so far; HashMapDecode
// is this repo's own HashMap<TKey,TValue> giving O(1) lookup instead. _targetShortUrl
// is deliberately the LAST-inserted pair so both strategies are forced through their
// full worst-case path (the same "force the real worst case" convention
// TwoSumBenchmarks already uses), not an early hit that would make the linear scan
// look artificially competitive.
[MemoryDiagnoser]
public class EncodeAndDecodeTinyURLBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private (string Short, string Long)[] _pairs = null!;
    private HashMap<string, string> _shortToLong = null!;
    private string _targetShortUrl = null!;

    [GlobalSetup]
    public void Setup()
    {
        _pairs = new (string, string)[Length];
        _shortToLong = new HashMap<string, string>();

        for (var i = 0; i < Length; i++)
        {
            var shortUrl = $"http://tinyurl.com/{i}";
            var longUrl = $"https://example.com/article/{i}";
            _pairs[i] = (shortUrl, longUrl);
            _shortToLong.Set(shortUrl, longUrl);
        }

        _targetShortUrl = $"http://tinyurl.com/{Length - 1}";
    }

    [Benchmark(Baseline = true)]
    public string LinearScanDecode()
    {
        foreach (var (shortUrl, longUrl) in _pairs)
        {
            if (shortUrl == _targetShortUrl)
            {
                return longUrl;
            }
        }

        return string.Empty;
    }

    [Benchmark]
    public string HashMapDecode()
    {
        _shortToLong.TryGetValue(_targetShortUrl, out var longUrl);
        return longUrl!;
    }
}
