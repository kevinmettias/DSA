using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindrome;

// Harness only. The two-pointer scan is ValidPalindromeSolution's; this file
// pins it to LeetCode's published examples.
public sealed class ValidPalindromeTests
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

    // One LeetCode example: the raw text, and whether it reads the same once only the
    // alphanumeric characters are kept and case is folded. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PalindromeCase(string Value, bool Expected);
}
