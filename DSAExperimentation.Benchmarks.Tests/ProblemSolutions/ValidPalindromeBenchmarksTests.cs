using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidPalindromeBenchmarks (ARCHITECTURE 17.9): the class carries a single
// arm, so there is no second strategy to reconcile and the assertion has to be an oracle rather than
// an agreement.
//
// It carries no [Params] and no [GlobalSetup] either - the fixed [Benchmark] operand is the whole
// workload - so a harness is nothing more than a new instance. That operand is LC 125's own first
// example, "A man, a plan, a canal: Panama", whose published answer is true: the punctuation and
// casing noise around a genuine palindrome is exactly what stops the two-pointer scan from resolving
// on the first pair. That literal is the decisive value asserted below.
public sealed partial class ValidPalindromeBenchmarksTests
{
    // LC 125's own first example: a true palindrome once the non-alphanumerics are skipped.
    private const bool ExpectedIsPalindrome = true;

    [Fact]
    public void IsPalindromeByTwoPointerScan_DocumentedValue_AcceptsTheExamplePalindrome() =>
        Assert.Equal(ExpectedIsPalindrome, BuildHarness().IsPalindromeByTwoPointerScan());

    private static ValidPalindromeBenchmarks BuildHarness() => new();
}
