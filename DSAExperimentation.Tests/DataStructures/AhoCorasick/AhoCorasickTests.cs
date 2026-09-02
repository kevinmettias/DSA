using DSAExperimentation.DataStructures.AhoCorasick;
using AhoCorasickAutomaton = DSAExperimentation.DataStructures.AhoCorasick.AhoCorasick;

namespace DSAExperimentation.Tests.DataStructures.AhoCorasick;

public sealed partial class AhoCorasickTests
{
    private const string PatternHe = "he";
    private const string PatternShe = "she";
    private const string PatternHis = "his";
    private const string PatternHers = "hers";
    private const string Ab = "ab";
    private const string PatternA = "a";
    private const string PatternXyz = "xyz";
    private const string TextAbcdef = "abcdef";
    private const string PatternAbc = "abc";
    private const string MixedCaseText = "XAbCX";

    [Fact]
    public void FindAll_ClassicExample_FindsAllOverlappingPatternsAcrossSharedText()
    {
        const string SearchText = "ushers";
        const int HeEndIndex = 2;
        const int HersEndIndex = 2;
        const int HersPatternIndex = 3;

        var automaton = new AhoCorasickAutomaton([PatternHe, PatternShe, PatternHis, PatternHers]);

        var matches = automaton.FindAll(SearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(1, 1), // "she"
                new AhoCorasickMatch(HeEndIndex, 0), // "he"
                new AhoCorasickMatch(HersEndIndex, HersPatternIndex), // "hers"
            ],
            matches);
    }

    // Exercises a genuinely deep, multi-level failure chain (a shallow/few-shared-prefix pattern
    // set would never walk GoTo's fallback loop more than one hop, or OutputLink's chain more than
    // one link).
    [Fact]
    public void FindAll_DeepFailureChain_FindsEveryPatternAcrossFallbackHops()
    {
        const string SearchText = "ahishers";
        const int HisPatternIndex = 3;
        const int SheEndIndex = 3;
        const int HeEndIndex = 4;
        const int HersEndIndex = 4;
        const int HersPatternIndex = 2;

        var automaton = new AhoCorasickAutomaton([PatternShe, PatternHe, PatternHers, PatternHis]);

        var matches = automaton.FindAll(SearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(1, HisPatternIndex), // "his"
                new AhoCorasickMatch(SheEndIndex, 0), // "she"
                new AhoCorasickMatch(HeEndIndex, 1), // "he"
                new AhoCorasickMatch(HersEndIndex, HersPatternIndex), // "hers"
            ],
            matches);
    }

    [Fact]
    public void FindAll_DuplicatePatterns_ReportsEachOccurrenceIndependently()
    {
        var automaton = new AhoCorasickAutomaton([Ab, Ab]);

        var matches = automaton.FindAll(Ab);

        Assert.Equal([new AhoCorasickMatch(0, 0), new AhoCorasickMatch(0, 1)], matches);
    }

    // Mixed empty/non-empty pattern set: the empty pattern must match at every insertion point
    // (0..text.Length inclusive) without swallowing or being swallowed by the non-empty pattern's
    // own matches.
    [Fact]
    public void FindAll_EmptyPatternMixedWithNonEmpty_MatchesEveryInsertionPointAndStillFindsRealMatches()
    {
        const string SearchText = "ba";

        var automaton = new AhoCorasickAutomaton([PatternA, string.Empty]);

        var matches = automaton.FindAll(SearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(0, 1), // ""
                new AhoCorasickMatch(1, 0), // "a"
                new AhoCorasickMatch(1, 1), // ""
                new AhoCorasickMatch(SearchText.Length, 1), // ""
            ],
            matches);
    }

    [Fact]
    public void FindAll_OnlyEmptyPattern_MatchesEveryInsertionPoint()
    {
        var automaton = new AhoCorasickAutomaton([string.Empty]);

        var matches = automaton.FindAll(Ab);

        Assert.Equal(
            [new AhoCorasickMatch(0, 0), new AhoCorasickMatch(1, 0), new AhoCorasickMatch(Ab.Length, 0)],
            matches);
    }

    [Fact]
    public void FindAll_NoPatterns_ReturnsEmptyList()
    {
        const string SearchText = "anything";

        var automaton = new AhoCorasickAutomaton([]);

        var matches = automaton.FindAll(SearchText);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var automaton = new AhoCorasickAutomaton([PatternXyz]);

        var matches = automaton.FindAll(TextAbcdef);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyText_ReturnsOnlyInsertionPointZeroMatches()
    {
        var automaton = new AhoCorasickAutomaton([PatternA, string.Empty]);

        var matches = automaton.FindAll(string.Empty);

        Assert.Equal([new AhoCorasickMatch(0, 1)], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var automaton = new AhoCorasickAutomaton([PatternAbc], caseInsensitive);

        var matches = automaton.FindAll(MixedCaseText);

        Assert.Equal([new AhoCorasickMatch(1, 0)], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var automaton = new AhoCorasickAutomaton([PatternAbc]);

        var matches = automaton.FindAll(MixedCaseText);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_SinglePattern_MatchesPrefixFunctionSearchAcrossVariedInputs()
    {
        const string TextHelloWorld = "hello world";
        const string PatternWorld = "world";
        const string TextAaaa = "aaaa";
        const string PatternAa = "aa";
        const string TextAabaab = "aabaab";
        const string PatternAab = "aab";

        (string Text, string Pattern)[] cases =
        [
            (TextHelloWorld, PatternWorld),
            (TextAaaa, PatternAa),
            (TextAabaab, PatternAab),
            (TextAbcdef, PatternXyz),
        ];

        foreach (var (text, pattern) in cases)
        {
            AssertFindAllMatchesPrefixFunctionSearch(text, pattern);
        }
    }

    private static void AssertFindAllMatchesPrefixFunctionSearch(string text, string pattern)
    {
        var automaton = new AhoCorasickAutomaton([pattern]);

        var expectedStarts = DSAExperimentation.Algorithms.StringMatching.PrefixFunctionSearch.FindAll(text, pattern);
        var actualStarts = automaton.FindAll(text).ConvertAll(match => match.Start);

        Assert.Equal(expectedStarts, actualStarts);
    }
}
