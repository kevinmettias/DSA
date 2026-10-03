using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidPalindromeSolution's, the same methods
// ValidPalindromeTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`)
// rather than a second strategy to reconcile. The operand is punctuation and
// casing noise around a true palindrome, so neither arm can resolve on the
// first pair.
public class ValidPalindromeBenchmarks
{
    // LeetCode 125's own first example: punctuation and casing noise around a
    // true palindrome, so the scan cannot short-circuit on the first pair.
    private const string Value = "A man, a plan, a canal: Panama";

    [Benchmark(Baseline = true)]
    public bool IsPalindromeByTwoPointerScan() => ValidPalindromeSolution.IsPalindromeByTwoPointerScan(Value);

    [Benchmark]
    public bool NormalizedReversal() => ValidPalindromeSolution.IsPalindromeByNormalizedReversal(Value);
}
