using DSAExperimentation.DataStructures.SuffixAutomaton;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;

namespace DSAExperimentation.Tests.DataStructures.SuffixAutomaton;

public sealed partial class SuffixAutomatonTests
{
    // Hand-verified worked example from the design pass: "abcbc" triggers two clone events,
    // including a clone becoming q in a later Extend call (exercising the walk-continues-from-a-
    // clone path). DistinctSubstringCount was independently cross-checked against brute-force
    // enumeration of all substrings of "abcbc".
    [Fact]
    public void DistinctSubstringCount_HandVerifiedWorkedExample_MatchesTracedValue()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        Assert.Equal(Fixtures.WorkedExampleDistinctSubstringCount, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void CountOccurrences_HandVerifiedWorkedExample_MatchesTracedSharedEndposState()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        // "c" (indices 2, 4) and "bc" (indices 1-2, 3-4) both land on the same traced state (7),
        // each occurring exactly twice.
        Assert.Equal(Fixtures.SharedEndposStateOccurrenceCount, automaton.CountOccurrences(Fixtures.TracedSubstringC));
        Assert.Equal(Fixtures.SharedEndposStateOccurrenceCount, automaton.CountOccurrences(Fixtures.TracedSubstringBc));
    }

    [Fact]
    public void HasSubstring_PresentSubstring_ReturnsTrue()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        Assert.True(automaton.HasSubstring(Fixtures.PresentSubstring));
    }

    [Fact]
    public void HasSubstring_AbsentSubstring_ReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        Assert.False(automaton.HasSubstring(Fixtures.AbsentSubstringXyz));
        Assert.False(automaton.HasSubstring(Fixtures.AbsentSubstringBa));
    }

    [Fact]
    public void CountOccurrences_AbsentPattern_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        Assert.Equal(0, automaton.CountOccurrences(Fixtures.AbsentSubstringXyz));
    }

    [Fact]
    public void CountOccurrences_EmptyPattern_ReturnsLengthPlusOne()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.WorkedExampleText);

        Assert.Equal(
            Fixtures.WorkedExampleEmptyPatternOccurrenceCount, automaton.CountOccurrences(Fixtures.EmptyString));
    }

    [Fact]
    public void HasSubstring_EmptyPattern_ReturnsTrueEvenForEmptyText()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.EmptyString);

        Assert.True(automaton.HasSubstring(Fixtures.EmptyString));
    }

    [Fact]
    public void HasSubstring_EmptyText_NonEmptyPatternReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.EmptyString);

        Assert.False(automaton.HasSubstring(Fixtures.SingleCharacterPattern));
    }

    [Fact]
    public void DistinctSubstringCount_EmptyText_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.EmptyString);

        Assert.Equal(0, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_AllDistinctCharacters_EqualsTriangularNumberOfLength()
    {
        var automaton = new SuffixAutomatonStructure(Fixtures.AllDistinctCharactersText);

        // n distinct-character text has exactly n*(n+1)/2 distinct substrings (every substring is
        // automatically unique when no character repeats).
        Assert.Equal(Fixtures.TriangularSubstringCountForLengthFive, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_MatchesBruteForceEnumerationAcrossVariedInputs()
    {
        foreach (var text in Fixtures.VariedTexts)
        {
            var automaton = new SuffixAutomatonStructure(text);

            Assert.Equal(BruteForceDistinctSubstringCount(text), automaton.DistinctSubstringCount);
        }
    }

    [Fact]
    public void CountOccurrences_MatchesBruteForceOccurrenceCountAcrossVariedSubstrings()
    {
        foreach (var text in Fixtures.OccurrenceCountTexts)
        {
            var automaton = new SuffixAutomatonStructure(text);

            foreach (var substring in AllDistinctSubstrings(text))
            {
                var expected = BruteForceCountOccurrences(text, new SearchPattern(substring));

                Assert.True(automaton.HasSubstring(substring));
                Assert.Equal(expected, automaton.CountOccurrences(substring));
            }
        }
    }

    private static long BruteForceDistinctSubstringCount(string text) => AllDistinctSubstrings(text).Count;

    private static HashSet<string> AllDistinctSubstrings(string text)
    {
        var substrings = new HashSet<string>();

        for (var start = 0; start < text.Length; start++)
        {
            for (var length = 1; start + length <= text.Length; length++)
            {
                var substring = text.Substring(start, length);
                substrings.Add(substring);
            }
        }

        return substrings;
    }

    // The pattern a brute-force cross-check counts occurrences of. Its own type because `text`
    // and `pattern` would otherwise both be `string`: the count is not symmetric, so
    // `BruteForceCountOccurrences(pattern, text)` would compile and quietly answer a different
    // question. With the role named, that call no longer compiles.
    private readonly record struct SearchPattern(string Value);

    private static int BruteForceCountOccurrences(string text, SearchPattern pattern)
    {
        var count = 0;

        for (var start = 0; start + pattern.Value.Length <= text.Length; start++)
        {
            if (text.AsSpan(start, pattern.Value.Length).SequenceEqual(pattern.Value))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// The texts these tests build automata from and the counts they expect back, named once so
    /// a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string WorkedExampleText = "abcbc";
        public const int WorkedExampleDistinctSubstringCount = 12;
        public const int SharedEndposStateOccurrenceCount = 2;
        public const string TracedSubstringC = "c";
        public const string TracedSubstringBc = "bc";
        public const string PresentSubstring = "cbc";
        public const string AbsentSubstringXyz = "xyz";
        public const string AbsentSubstringBa = "ba";
        public const int WorkedExampleEmptyPatternOccurrenceCount = 6;
        public const string EmptyString = "";
        public const string SingleCharacterPattern = "a";
        public const string AllDistinctCharactersText = "abcde";
        public const int TriangularSubstringCountForLengthFive = 15;
        public const string BananaText = "banana";
        public const string RepeatedACharacterText = "aaaa";
        public const string MississippiText = "mississippi";
        public const string NearRepeatText = "abcabd";
        public const string NestedRepeatText = "aabaabaaab";

        // Repeat-heavy and clone-triggering texts, so the brute-force cross-checks stress the
        // automaton's own bookkeeping rather than only its happy path.
        public static readonly string[] VariedTexts =
            [WorkedExampleText, BananaText, RepeatedACharacterText, MississippiText, NearRepeatText, NestedRepeatText];

        public static readonly string[] OccurrenceCountTexts =
            [WorkedExampleText, BananaText, RepeatedACharacterText, MississippiText, NearRepeatText];
    }
}
