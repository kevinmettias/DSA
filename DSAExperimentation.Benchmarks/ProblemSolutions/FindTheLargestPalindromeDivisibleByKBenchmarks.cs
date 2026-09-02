using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheLargestPalindromeDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheLargestPalindromeDivisibleByKSolution's, the
// same methods FindTheLargestPalindromeDivisibleByKTests proves correct. N stays
// small enough for the brute-force arm to finish in reasonable time (h = ceil(N/2)
// half-digits, 9*10^(h-1) leaves) - the digit-DP arm's whole point is that it does
// not care how large N gets, only h*k states.
[MemoryDiagnoser]
public class FindTheLargestPalindromeDivisibleByKBenchmarks
{
    private const int K = 7;

    [Params(5, 9)]
    public int N;

    [Benchmark(Baseline = true)]
    public string BruteForce() => FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByBruteForce(N, K);

    [Benchmark]
    public string DigitDpMemo() => FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByDigitDpMemo(N, K);
}
