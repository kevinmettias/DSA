using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrimePalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrimePalindromeSolution's. LowerBound=999 lands on a
// large palindrome-sparse stretch - the next prime palindrome is 10301, since no
// four-digit palindrome is ever prime - so the sequential scan pays for ~9,300
// candidates the generator never visits.
[MemoryDiagnoser]
public class PrimePalindromeBenchmarks
{
    [Params(13, 999)]
    public int LowerBound { get; set; }

    [Benchmark(Baseline = true)]
    public long SequentialScan() =>
        PrimePalindromeSolution.SmallestPrimePalindromeBySequentialScan(LowerBound);

    [Benchmark]
    public long PalindromeGeneration() =>
        PrimePalindromeSolution.SmallestPrimePalindromeByPalindromeGeneration(LowerBound);
}
