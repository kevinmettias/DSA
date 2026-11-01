using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidPalindromeBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong verdict would pass, so this pins the
// baseline's verdict to the one the workload's construction fixes.
//
// Setup's operand comes from ValidPalindromeWorkloads, which builds punctuation and casing noise
// around a mirrored run of letters and digits - a shape its own tests pin - so its decisive answer
// is true, the same verdict LC 125's own first example has. The noise is exactly what stops the
// two-pointer scan from resolving on the first pair.
public sealed partial class ValidPalindromeBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // A mirrored core of letters and digits is a palindrome once the noise around it is skipped.
    private const bool ExpectedIsPalindrome = true;

    [Fact]
    public void IsPalindromeByTwoPointerScan_NoisyPalindrome_AcceptsIt() =>
        Assert.Equal(ExpectedIsPalindrome, BuildHarness().IsPalindromeByTwoPointerScan());

    private static ValidPalindromeBenchmarks BuildHarness()
    {
        var harness = new ValidPalindromeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
