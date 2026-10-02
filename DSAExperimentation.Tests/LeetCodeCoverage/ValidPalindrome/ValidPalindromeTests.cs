using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindrome;

// Harness only. Both strategies are ValidPalindromeSolution's; this file pins
// them to LeetCode's published examples.
public sealed partial class ValidPalindromeTests
{
    public static TheoryData<PalindromeCase> Examples =>
        new()
        {
            { new PalindromeCase(Value: "A man, a plan, a canal: Panama", Expected: true) },
            { new PalindromeCase(Value: "race a car", Expected: false) },
            { new PalindromeCase(Value: " ", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByTwoPointerScan_LeetCodeExamples_IgnoresNonAlphanumeric(PalindromeCase example)
    {
        var isPalindrome = ValidPalindromeSolution.IsPalindromeByTwoPointerScan(example.Value);

        Assert.Equal(example.Expected, isPalindrome);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByNormalizedReversal_LeetCodeExamples_IgnoresNonAlphanumeric(PalindromeCase example)
    {
        var isPalindrome = ValidPalindromeSolution.IsPalindromeByNormalizedReversal(example.Value);

        Assert.Equal(example.Expected, isPalindrome);
    }

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they return the same verdict on every example - not merely
    // that each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindrome_AgreeOnEveryExample(PalindromeCase example) =>
        Assert.Equal(
            ValidPalindromeSolution.IsPalindromeByTwoPointerScan(example.Value),
            ValidPalindromeSolution.IsPalindromeByNormalizedReversal(example.Value));

    // One LeetCode example: the raw text, and whether it reads the same once only the
    // alphanumeric characters are kept and case is folded. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PalindromeCase(string Value, bool Expected);
}
