using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrimePalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrimePalindromeSolution's. N=999 lands on a large
// palindrome-sparse stretch - the next prime palindrome is 10301, since no four-digit
// palindrome is ever prime - so the sequential scan pays for ~9,300 candidates the
// generator never visits.
[MemoryDiagnoser]
public class PrimePalindromeBenchmarks
{
    [Params(13, 999)]
    public int N;

    [Benchmark(Baseline = true)]
    public long SequentialScan() => PrimePalindromeSolution.SmallestPrimePalindromeBySequentialScan(N);

    [Benchmark]
    public long PalindromeGeneration() => PrimePalindromeSolution.SmallestPrimePalindromeByPalindromeGeneration(N);
}
