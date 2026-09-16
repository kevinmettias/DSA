using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountKReducibleNumbersLessThanN;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountKReducibleNumbersLessThanNSolution's, the
// same methods CountKReducibleNumbersLessThanNTests proves correct
// (CountAnagramsBenchmarks precedent). The binary string always starts with '1'
// (no leading zeros, matching LC's own constraint) and the rest is random, so n
// is a genuinely mixed bit pattern rather than a power of two. Length stays
// small - brute force enumerates every one of the [1, n) integers it counts, so
// it would not finish at the real problem's 800-bit scale, even though the
// combinatorial arm scales to it trivially (CountAnagramsBenchmarks' word-length
// tradeoff, here on bit length instead).
[MemoryDiagnoser]
public class CountKReducibleNumbersLessThanNBenchmarks
{
    private const int Seed = 3352; // LC problem number
    private const int K = 3;

    private string _binaryDigits = "";

    [Params(16, 24)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var bits = new char[Length];
        bits[0] = '1';

        for (var i = 1; i < Length; i++)
        {
            var isZeroBit = random.Next(2) == 0;
            bits[i] = isZeroBit ? '0' : '1';
        }

        _binaryDigits = new string(bits);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        CountKReducibleNumbersLessThanNSolution.CountKReducibleNumbersByBruteForce(_binaryDigits, K);

    [Benchmark]
    public int PopcountCombinatorics() =>
        CountKReducibleNumbersLessThanNSolution.CountKReducibleNumbersByPopcountCombinatorics(_binaryDigits, K);
}
