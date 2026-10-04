using DSAExperimentation.LeetCode.ValidPalindromeII;

namespace DSAExperimentation.LeetCode.Tests.ValidPalindromeII;

// Harness only. Both strategies are ValidPalindromeIISolution's - this file
// just pins them to LeetCode's published examples and one longer non-palindrome.
public sealed partial class ValidPalindromeIISolutionTests
{
    public static TheoryData<PalindromeExample> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            new PalindromeExample("aba", IsValidAfterAtMostOneDeletion: true),
            new PalindromeExample("abca", IsValidAfterAtMostOneDeletion: true),
            new PalindromeExample("abc", IsValidAfterAtMostOneDeletion: false),

            // "abcdef" mismatches a/f, and dropping either leaves b/f or a/e mismatched.
            new PalindromeExample("abcdef", IsValidAfterAtMostOneDeletion: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidPalindromeByBruteForceDeletion_LeetCodeExamples_ReturnsWhetherAtMostOneDeletionWorks(
        PalindromeExample example)
    {
        var actual = ValidPalindromeIISolution.IsValidPalindromeByBruteForceDeletion(example.Text);

        Assert.Equal(example.IsValidAfterAtMostOneDeletion, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidPalindromeByMismatchSkip_LeetCodeExamples_ReturnsWhetherAtMostOneDeletionWorks(
        PalindromeExample example)
    {
        var actual = ValidPalindromeIISolution.IsValidPalindromeByMismatchSkip(example.Text);

        Assert.Equal(example.IsValidAfterAtMostOneDeletion, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PalindromeExample(string Text, bool IsValidAfterAtMostOneDeletion);
}
