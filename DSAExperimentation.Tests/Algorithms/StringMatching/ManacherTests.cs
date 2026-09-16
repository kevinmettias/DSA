using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.Algorithms.StringMatching;

public sealed partial class ManacherTests
{
    private static readonly string[] BruteForceCrossCheckStrings =
    [
        "aaaa", "abcba", "aabbaa", "racecar", "abacabad", "mississippi", "abababab", "x", "",
    ];

    // The obviously-correct O(n^2) reference: expand around each center by hand.
    // Cross-checking Manacher's O(n) result against this over several varied,
    // repeat-heavy strings is a stronger correctness proof than a handful of
    // manually hand-traced cases alone - it catches indexing mistakes a couple of
    // small hand-picked examples could accidentally miss.
    private static int[] BruteForceOddRadii(string text)
    {
        var radii = new int[text.Length];

        for (var i = 0; i < text.Length; i++)
        {
            var k = 1;

            while (OddPalindromeGrowsAt(text, i, k))
            {
                k++;
            }

            radii[i] = k;
        }

        return radii;
    }

    private static int[] BruteForceEvenRadii(string text)
    {
        var radii = new int[text.Length];

        for (var i = 0; i < text.Length; i++)
        {
            var k = 0;

            while (EvenPalindromeGrowsAt(text, i, k))
            {
                k++;
            }

            radii[i] = k;
        }

        return radii;
    }

    // Whether the odd palindrome centred on a character still grows: the mirrored pair
    // one step further out both exists inside the text and matches.
    private static bool OddPalindromeGrowsAt(string text, int center, int radius)
        => center - radius >= 0
            && center + radius < text.Length
            && text[center - radius] == text[center + radius];

    // The even-radius twin: the mirrored pair straddling the gap before the center.
    private static bool EvenPalindromeGrowsAt(string text, int center, int radius)
        => center - radius - 1 >= 0
            && center + radius < text.Length
            && text[center - radius - 1] == text[center + radius];

    [Fact]
    public void ComputeOddRadii_MatchesBruteForceExpansionAcrossVariedStrings()
    {
        foreach (var text in BruteForceCrossCheckStrings)
        {
            Assert.Equal(BruteForceOddRadii(text), Manacher.ComputeOddRadii(text));
        }
    }

    [Fact]
    public void ComputeEvenRadii_MatchesBruteForceExpansionAcrossVariedStrings()
    {
        foreach (var text in BruteForceCrossCheckStrings)
        {
            Assert.Equal(BruteForceEvenRadii(text), Manacher.ComputeEvenRadii(text));
        }
    }

    // "babad": hand-verified - center-index 1 ('a') gives odd radius 2, i.e.
    // text[0..3) = "bab".
    [Fact]
    public void ComputeOddRadii_Babad_MatchesHandVerifiedValues()
        => Assert.Equal(new[] { 1, 2, 2, 1, 1 }, Manacher.ComputeOddRadii("babad"));

    // "cbbd": hand-verified - the only even palindrome is "bb" at index 2, radius 1.
    [Fact]
    public void ComputeEvenRadii_Cbbd_MatchesHandVerifiedValues()
        => Assert.Equal(new[] { 0, 0, 1, 0 }, Manacher.ComputeEvenRadii("cbbd"));

    [Fact]
    public void FindLongestPalindromicSubstring_Babad_ReturnsBab()
        => Assert.Equal((0, 3), Manacher.FindLongestPalindromicSubstring("babad"));

    [Fact]
    public void FindLongestPalindromicSubstring_Cbbd_ReturnsBb()
        => Assert.Equal((1, 2), Manacher.FindLongestPalindromicSubstring("cbbd"));

    [Fact]
    public void FindLongestPalindromicSubstring_WholeStringIsAPalindrome_ReturnsWholeString()
        => Assert.Equal((0, 7), Manacher.FindLongestPalindromicSubstring("racecar"));

    [Fact]
    public void FindLongestPalindromicSubstring_SingleCharacter_ReturnsThatCharacter()
        => Assert.Equal((0, 1), Manacher.FindLongestPalindromicSubstring("x"));

    [Fact]
    public void FindLongestPalindromicSubstring_EmptyText_ReturnsZeroLength()
        => Assert.Equal((0, 0), Manacher.FindLongestPalindromicSubstring(""));

    [Fact]
    public void FindLongestPalindromicSubstring_WithCustomComparer_IsCaseInsensitive()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var longest = Manacher.FindLongestPalindromicSubstring("BaB", caseInsensitive);

        Assert.Equal((0, 3), longest);
    }
}
