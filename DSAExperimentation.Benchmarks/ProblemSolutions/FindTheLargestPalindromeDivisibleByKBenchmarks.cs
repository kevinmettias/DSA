using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheLargestPalindromeDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheLargestPalindromeDivisibleByKSolution's, the
// same methods FindTheLargestPalindromeDivisibleByKTests proves correct. DigitCount
// stays small enough for the brute-force arm to finish in reasonable time
// (halfLength = ceil(DigitCount/2) half-digits, 9*10^(halfLength-1) leaves) - the
// digit-DP arm's whole point is that it does not care how large DigitCount gets,
// only halfLength*K states.
[MemoryDiagnoser]
public class FindTheLargestPalindromeDivisibleByKBenchmarks
{
    private const int K = 7;

    [Params(5, 9)]
    public int DigitCount { get; set; }

    [Benchmark(Baseline = true)]
    public string BruteForce() =>
        FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByBruteForce(DigitCount, K);

    [Benchmark]
    public string DigitDpMemo() =>
        FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByDigitDpMemo(DigitCount, K);
}
