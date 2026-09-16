using DSAExperimentation.LeetCode.ValidPalindromeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindromeII;

// Harness only. Both strategies are ValidPalindromeIISolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class ValidPalindromeIITests
{
    public static TheoryData<PalindromeExample> Examples =>
        new()
        {
            new PalindromeExample("aba", IsValidAfterAtMostOneDeletion: true),
            new PalindromeExample("abca", IsValidAfterAtMostOneDeletion: true),
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
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PalindromeExample(string Text, bool IsValidAfterAtMostOneDeletion);
}
