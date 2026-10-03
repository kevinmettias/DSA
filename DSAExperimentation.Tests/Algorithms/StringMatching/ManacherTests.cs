using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.Tests.Algorithms.StringMatching.Fixtures;

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

            while (IsOddPalindromeGrowingAt(text, i, k))
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

            while (IsEvenPalindromeGrowingAt(text, i, k))
            {
                k++;
            }

            radii[i] = k;
        }

        return radii;
    }

    // Whether the odd palindrome centred on a character still grows: the mirrored pair
    // one step further out both exists inside the text and matches.
    private static bool IsOddPalindromeGrowingAt(string text, int center, int radius)
        => center - radius >= 0
            && center + radius < text.Length
            && text[center - radius] == text[center + radius];

    // The even-radius twin: the mirrored pair straddling the gap before the center.
    private static bool IsEvenPalindromeGrowingAt(string text, int center, int radius)
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

    // "cbbd": hand-verified - the only even palindrome it contains is its central pair
    // "bb", at index 2, radius 1.
    [Fact]
    public void ComputeEvenRadii_CentralPairOnly_MatchesHandVerifiedValues()
        => Assert.Equal(new[] { 0, 0, 1, 0 }, Manacher.ComputeEvenRadii("cbbd"));

    [Fact]
    public void FindLongestPalindromicSubstring_Babad_ReturnsBab()
        => Assert.Equal((0, 3), Manacher.FindLongestPalindromicSubstring("babad"));

    // "cbbd": hand-verified - the longest palindromic substring is that same central
    // pair "bb", half-open (1, 2).
    [Fact]
    public void FindLongestPalindromicSubstring_CentralPairOnly_ReturnsThatPair()
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

    // RightmostPalindromeEdge is private to Manacher. ExpandTo is what keeps both radius
    // scans linear: moving the edge only when a palindrome reaches further right is what
    // lets each later center seed from its mirror instead of expanding from scratch. The
    // radii come out right either way, so the comparer's call count - not the answer -
    // is what shows whether ExpandTo did its job. Each comparison either pushes the edge
    // right (at most once per character) or ends one center's expansion (at most once per
    // center), which is the two-per-character bound asserted below.
    public sealed partial class RightmostPalindromeEdgeTests
    {
        // Repeating "aab" defeats both ways ExpandTo could go wrong: an edge that never
        // moves re-expands every center from scratch, and an edge that also moves when the
        // new palindrome falls short retreats over ground already covered. Either one
        // costs more than ten times the bound.
        private static readonly string RepeatedAab = string.Concat(Enumerable.Repeat("aab", 100));

        [Fact]
        public void ExpandTo_KeepsTheOddScanWithinTwoComparisonsPerCharacter()
        {
            var comparer = new CountingCharComparer();

            Manacher.ComputeOddRadii(RepeatedAab, comparer);

            Assert.InRange(comparer.Comparisons, 1, 2 * RepeatedAab.Length);
        }

        [Fact]
        public void ExpandTo_KeepsTheEvenScanWithinTwoComparisonsPerCharacter()
        {
            var comparer = new CountingCharComparer();

            Manacher.ComputeEvenRadii(RepeatedAab, comparer);

            Assert.InRange(comparer.Comparisons, 1, 2 * RepeatedAab.Length);
        }
    }
}
