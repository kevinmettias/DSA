using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidPalindromeBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the two-pointer scan and the normalized-reversal copy - so a
// harness whose arms disagree is classifying two different strings.
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

    [Fact]
    public void NormalizedReversal_DocumentedValue_AcceptsTheExamplePalindrome() =>
        Assert.Equal(ExpectedIsPalindrome, BuildHarness().NormalizedReversal());

    [Fact]
    public void NormalizedReversal_AgreesWithIsPalindromeByTwoPointerScan() =>
        Assert.Equal(BuildHarness().IsPalindromeByTwoPointerScan(), BuildHarness().NormalizedReversal());

    private static ValidPalindromeBenchmarks BuildHarness() => new();
}
