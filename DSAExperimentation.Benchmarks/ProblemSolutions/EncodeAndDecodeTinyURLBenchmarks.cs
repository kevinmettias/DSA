using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EncodeAndDecodeTinyURL;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EncodeAndDecodeTinyURLSolution's, the same classes
// EncodeAndDecodeTinyURLTests proves correct. [GlobalSetup] populates each strategy
// via its own Encode, so build cost is charged to setup rather than to the Decode
// each [Benchmark] arm measures. _targetShortUrl is deliberately the
// LAST-inserted pair so both strategies are forced through their full worst-case
// Decode path (the same "force the real worst case" convention TwoSumBenchmarks
// already uses), not an early hit that would make the linear scan look
// artificially competitive.
[MemoryDiagnoser]
public class EncodeAndDecodeTinyURLBenchmarks
{
    private EncodeAndDecodeTinyURLSolution.CodecByLinearScan _linearScan = new();

    private EncodeAndDecodeTinyURLSolution.CodecByHashMap _hashMap = new();
    private string _targetShortUrl = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _linearScan = new EncodeAndDecodeTinyURLSolution.CodecByLinearScan();
        _hashMap = new EncodeAndDecodeTinyURLSolution.CodecByHashMap();

        for (var i = 0; i < Length; i++)
        {
            var longUrl = $"https://example.com/article/{i}";
            _linearScan.Encode(longUrl);
            _hashMap.Encode(longUrl);
        }

        _targetShortUrl = $"http://tinyurl.com/{Length - 1}";
    }

    [Benchmark(Baseline = true)]
    public string LinearScanDecode() => _linearScan.Decode(_targetShortUrl);

    [Benchmark]
    public string HashMapDecode() => _hashMap.Decode(_targetShortUrl);
}
