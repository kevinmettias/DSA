using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestPalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestPalindromeSolution's, the same methods
// ShortestPalindromeTests proves correct. Random lowercase letters give s no long
// palindromic prefix, so both strategies are forced through nearly their full
// worst-case scan instead of an early exit making the naive version look
// artificially competitive.
[MemoryDiagnoser]
public class ShortestPalindromeBenchmarks
{
    private const int AlphabetSize = 26;

    [Params(200, 2_000)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        _value = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string NaivePrefixScan() => ShortestPalindromeSolution.BuildShortestPalindromeByNaiveScan(_value);

    [Benchmark]
    public string KmpFailureFunction() =>
        ShortestPalindromeSolution.BuildShortestPalindromeByKmpFailureFunction(_value);
}
