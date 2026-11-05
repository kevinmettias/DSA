using DSAExperimentation.LeetCode.Solutions.ValidPalindromeII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidPalindromeII;

// Harness only. Both strategies are ValidPalindromeIISolution's - this file
// just pins them to one longer non-palindrome. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ValidPalindromeIISolutionTests
{
    public static TheoryData<PalindromeCase> Cases =>
        new()
        {
            // "abcdef" mismatches a/f, and dropping either leaves b/f or a/e mismatched.
            new PalindromeCase("abcdef", IsValidAfterAtMostOneDeletion: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidPalindromeByBruteForceDeletion_Cases_ReturnsWhetherAtMostOneDeletionWorks(
        PalindromeCase testCase)
    {
        var actual = ValidPalindromeIISolution.IsValidPalindromeByBruteForceDeletion(testCase.Text);

        Assert.Equal(testCase.IsValidAfterAtMostOneDeletion, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidPalindromeByMismatchSkip_Cases_ReturnsWhetherAtMostOneDeletionWorks(
        PalindromeCase testCase)
    {
        var actual = ValidPalindromeIISolution.IsValidPalindromeByMismatchSkip(testCase.Text);

        Assert.Equal(testCase.IsValidAfterAtMostOneDeletion, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PalindromeCase(string Text, bool IsValidAfterAtMostOneDeletion);
}
