using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ValidPalindromeSolution's, the same method
// ValidPalindromeTests proves correct. Pre-migration this class was an
// untested compile-smoke placeholder (`Baseline() => 1`,
// `PrimitiveComposed() => 1`) rather than a second strategy to reconcile.
[MemoryDiagnoser]
public class ValidPalindromeBenchmarks
{
    // LeetCode 125's own first example: punctuation and casing noise around a
    // true palindrome, so the scan cannot short-circuit on the first pair.
    private const string Value = "A man, a plan, a canal: Panama";

    [Benchmark(Baseline = true)]
    public bool TwoPointerScan() => ValidPalindromeSolution.IsPalindromeByTwoPointerScan(Value);
}
