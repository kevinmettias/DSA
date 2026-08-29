using DSAExperimentation.DataStructures.AhoCorasick;
using AhoCorasickAutomaton = DSAExperimentation.DataStructures.AhoCorasick.AhoCorasick;

namespace DSAExperimentation.Tests.DataStructures.AhoCorasick;

public sealed partial class AhoCorasickTests
{
    [Fact]
    public void FindAll_ClassicExample_FindsAllOverlappingPatternsAcrossSharedText()
    {
        var automaton = new AhoCorasickAutomaton(["he", "she", "his", "hers"]);

        var matches = automaton.FindAll("ushers");

        Assert.Equal(
            [
                new AhoCorasickMatch(1, 1), // "she"
                new AhoCorasickMatch(2, 0), // "he"
                new AhoCorasickMatch(2, 3), // "hers"
            ],
            matches);
    }

    // Exercises a genuinely deep, multi-level failure chain (a shallow/few-shared-prefix pattern
    // set would never walk GoTo's fallback loop more than one hop, or OutputLink's chain more than
    // one link).
    [Fact]
    public void FindAll_DeepFailureChain_FindsEveryPatternAcrossFallbackHops()
    {
        var automaton = new AhoCorasickAutomaton(["she", "he", "hers", "his"]);

        var matches = automaton.FindAll("ahishers");

        Assert.Equal(
            [
                new AhoCorasickMatch(1, 3), // "his"
                new AhoCorasickMatch(3, 0), // "she"
                new AhoCorasickMatch(4, 1), // "he"
                new AhoCorasickMatch(4, 2), // "hers"
            ],
            matches);
    }

    [Fact]
    public void FindAll_DuplicatePatterns_ReportsEachOccurrenceIndependently()
    {
        var automaton = new AhoCorasickAutomaton(["ab", "ab"]);

        var matches = automaton.FindAll("ab");

        Assert.Equal([new AhoCorasickMatch(0, 0), new AhoCorasickMatch(0, 1)], matches);
    }

    // Mixed empty/non-empty pattern set: the empty pattern must match at every insertion point
    // (0..text.Length inclusive) without swallowing or being swallowed by the non-empty pattern's
    // own matches.
    [Fact]
    public void FindAll_EmptyPatternMixedWithNonEmpty_MatchesEveryInsertionPointAndStillFindsRealMatches()
    {
        var automaton = new AhoCorasickAutomaton(["a", ""]);

        var matches = automaton.FindAll("ba");

        Assert.Equal(
            [
                new AhoCorasickMatch(0, 1), // ""
                new AhoCorasickMatch(1, 0), // "a"
                new AhoCorasickMatch(1, 1), // ""
                new AhoCorasickMatch(2, 1), // ""
            ],
            matches);
    }

    [Fact]
    public void FindAll_OnlyEmptyPattern_MatchesEveryInsertionPoint()
    {
        var automaton = new AhoCorasickAutomaton([""]);

        var matches = automaton.FindAll("ab");

        Assert.Equal([new AhoCorasickMatch(0, 0), new AhoCorasickMatch(1, 0), new AhoCorasickMatch(2, 0)], matches);
    }

    [Fact]
    public void FindAll_NoPatterns_ReturnsEmptyList()
    {
        var automaton = new AhoCorasickAutomaton([]);

        var matches = automaton.FindAll("anything");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var automaton = new AhoCorasickAutomaton(["xyz"]);

        var matches = automaton.FindAll("abcdef");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyText_ReturnsOnlyInsertionPointZeroMatches()
    {
        var automaton = new AhoCorasickAutomaton(["a", ""]);

        var matches = automaton.FindAll("");

        Assert.Equal([new AhoCorasickMatch(0, 1)], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var automaton = new AhoCorasickAutomaton(["abc"], caseInsensitive);

        var matches = automaton.FindAll("XAbCX");

        Assert.Equal([new AhoCorasickMatch(1, 0)], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var automaton = new AhoCorasickAutomaton(["abc"]);

        var matches = automaton.FindAll("XAbCX");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_SinglePattern_MatchesPrefixFunctionSearchAcrossVariedInputs()
    {
        (string Text, string Pattern)[] cases =
        [
            ("hello world", "world"),
            ("aaaa", "aa"),
            ("aabaab", "aab"),
            ("abcdef", "xyz"),
        ];

        foreach (var (text, pattern) in cases)
        {
            var automaton = new AhoCorasickAutomaton([pattern]);

            var expectedStarts = DSAExperimentation.Algorithms.StringMatching.PrefixFunctionSearch.FindAll(text, pattern);
            var actualStarts = automaton.FindAll(text).ConvertAll(match => match.Start);

            Assert.Equal(expectedStarts, actualStarts);
        }
    }
}
