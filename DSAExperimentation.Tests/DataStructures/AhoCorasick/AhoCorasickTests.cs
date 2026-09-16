using DSAExperimentation.DataStructures.AhoCorasick;
using AhoCorasickAutomaton = DSAExperimentation.DataStructures.AhoCorasick.AhoCorasick;

namespace DSAExperimentation.Tests.DataStructures.AhoCorasick;

public sealed partial class AhoCorasickTests
{
    [Fact]
    public void FindAll_ClassicExample_FindsAllOverlappingPatternsAcrossSharedText()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternHe, Fixtures.PatternShe, Fixtures.PatternHis, Fixtures.PatternHers]);

        var matches = automaton.FindAll(Fixtures.UshersSearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(1, 1), // "she"
                new AhoCorasickMatch(Fixtures.UshersHeEndIndex, 0), // "he"
                new AhoCorasickMatch(Fixtures.UshersHersEndIndex, Fixtures.UshersHersPatternIndex), // "hers"
            ],
            matches);
    }

    // Exercises a genuinely deep, multi-level failure chain (a shallow/few-shared-prefix pattern
    // set would never walk GoTo's fallback loop more than one hop, or OutputLink's chain more than
    // one link).
    [Fact]
    public void FindAll_DeepFailureChain_FindsEveryPatternAcrossFallbackHops()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternShe, Fixtures.PatternHe, Fixtures.PatternHers, Fixtures.PatternHis]);

        var matches = automaton.FindAll(Fixtures.AhishersSearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(1, Fixtures.AhishersHisPatternIndex), // "his"
                new AhoCorasickMatch(Fixtures.AhishersSheEndIndex, 0), // "she"
                new AhoCorasickMatch(Fixtures.AhishersHeEndIndex, 1), // "he"
                new AhoCorasickMatch(Fixtures.AhishersHersEndIndex, Fixtures.AhishersHersPatternIndex), // "hers"
            ],
            matches);
    }

    [Fact]
    public void FindAll_DuplicatePatterns_ReportsEachOccurrenceIndependently()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.Ab, Fixtures.Ab]);

        var matches = automaton.FindAll(Fixtures.Ab);

        Assert.Equal([new AhoCorasickMatch(0, 0), new AhoCorasickMatch(0, 1)], matches);
    }

    // Mixed empty/non-empty pattern set: the empty pattern must match at every insertion point
    // (0..text.Length inclusive) without swallowing or being swallowed by the non-empty pattern's
    // own matches.
    [Fact]
    public void FindAll_EmptyPatternMixedWithNonEmpty_MatchesEveryInsertionPointAndStillFindsRealMatches()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternA, string.Empty]);

        var matches = automaton.FindAll(Fixtures.BaSearchText);

        Assert.Equal(
            [
                new AhoCorasickMatch(0, 1), // ""
                new AhoCorasickMatch(1, 0), // "a"
                new AhoCorasickMatch(1, 1), // ""
                new AhoCorasickMatch(Fixtures.BaSearchText.Length, 1), // ""
            ],
            matches);
    }

    [Fact]
    public void FindAll_OnlyEmptyPattern_MatchesEveryInsertionPoint()
    {
        var automaton = new AhoCorasickAutomaton([string.Empty]);

        var matches = automaton.FindAll(Fixtures.Ab);

        Assert.Equal(
            [new AhoCorasickMatch(0, 0), new AhoCorasickMatch(1, 0), new AhoCorasickMatch(Fixtures.Ab.Length, 0)],
            matches);
    }

    [Fact]
    public void FindAll_NoPatterns_ReturnsEmptyList()
    {
        var automaton = new AhoCorasickAutomaton([]);

        var matches = automaton.FindAll(Fixtures.AnythingSearchText);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternXyz]);

        var matches = automaton.FindAll(Fixtures.TextAbcdef);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyText_ReturnsOnlyInsertionPointZeroMatches()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternA, string.Empty]);

        var matches = automaton.FindAll(string.Empty);

        Assert.Equal([new AhoCorasickMatch(0, 1)], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var automaton = new AhoCorasickAutomaton([Fixtures.PatternAbc], caseInsensitive);

        var matches = automaton.FindAll(Fixtures.MixedCaseText);

        Assert.Equal([new AhoCorasickMatch(1, 0)], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var automaton = new AhoCorasickAutomaton([Fixtures.PatternAbc]);

        var matches = automaton.FindAll(Fixtures.MixedCaseText);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_SinglePattern_MatchesPrefixFunctionSearchAcrossVariedInputs()
    {
        (string Text, string Pattern)[] cases =
        [
            (Fixtures.HelloWorldText, Fixtures.WorldPattern),
            (Fixtures.RepeatedPairText, Fixtures.RepeatedPairPattern),
            (Fixtures.SelfOverlappingText, Fixtures.SelfOverlappingPattern),
            (Fixtures.TextAbcdef, Fixtures.PatternXyz),
        ];

        foreach (var searchCase in cases)
        {
            AssertFindAllMatchesPrefixFunctionSearch(searchCase);
        }
    }

    // The pair travels as one value: passed as two adjacent strings, a transposed call
    // site would compile and silently search for the text inside the pattern.
    private static void AssertFindAllMatchesPrefixFunctionSearch((string Text, string Pattern) searchCase)
    {
        var automaton = new AhoCorasickAutomaton([searchCase.Pattern]);

        var expectedStarts = DSAExperimentation.Algorithms.StringMatching.PrefixFunctionSearch.FindAll(searchCase.Text, searchCase.Pattern);
        var actualStarts = automaton.FindAll(searchCase.Text).ConvertAll(match => match.Start);

        Assert.Equal(expectedStarts, actualStarts);
    }

    /// <summary>
    /// The texts and patterns these tests search, and the match indices they expect
    /// back, named once so a second test does not have to reach into a neighbour's
    /// body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string PatternHe = "he";
        public const string PatternShe = "she";
        public const string PatternHis = "his";
        public const string PatternHers = "hers";
        public const string Ab = "ab";
        public const string PatternA = "a";
        public const string PatternXyz = "xyz";
        public const string TextAbcdef = "abcdef";
        public const string PatternAbc = "abc";
        public const string MixedCaseText = "XAbCX";

        public const string UshersSearchText = "ushers";
        public const int UshersHeEndIndex = 2;
        public const int UshersHersEndIndex = 2;
        public const int UshersHersPatternIndex = 3;

        public const string AhishersSearchText = "ahishers";
        public const int AhishersHisPatternIndex = 3;
        public const int AhishersSheEndIndex = 3;
        public const int AhishersHeEndIndex = 4;
        public const int AhishersHersEndIndex = 4;
        public const int AhishersHersPatternIndex = 2;

        public const string BaSearchText = "ba";
        public const string AnythingSearchText = "anything";

        public const string HelloWorldText = "hello world";
        public const string WorldPattern = "world";
        public const string RepeatedPairText = "aaaa";
        public const string RepeatedPairPattern = "aa";
        public const string SelfOverlappingText = "aabaab";
        public const string SelfOverlappingPattern = "aab";
    }
}
