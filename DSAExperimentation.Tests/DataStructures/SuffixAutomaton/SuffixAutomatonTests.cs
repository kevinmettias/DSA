using DSAExperimentation.DataStructures.SuffixAutomaton;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;

namespace DSAExperimentation.Tests.DataStructures.SuffixAutomaton;

public sealed partial class SuffixAutomatonTests
{
    private const string WorkedExampleText = "abcbc";
    private const int WorkedExampleDistinctSubstringCount = 12;
    private const int SharedEndposStateOccurrenceCount = 2;
    private const string TracedSubstringC = "c";
    private const string TracedSubstringBc = "bc";
    private const string PresentSubstring = "cbc";
    private const string AbsentSubstringXyz = "xyz";
    private const string AbsentSubstringBa = "ba";
    private const int WorkedExampleEmptyPatternOccurrenceCount = 6;
    private const string EmptyString = "";
    private const string SingleCharacterPattern = "a";
    private const string AllDistinctCharactersText = "abcde";
    private const int TriangularSubstringCountForLengthFive = 15;
    private const string BananaText = "banana";
    private const string RepeatedACharacterText = "aaaa";
    private const string MississippiText = "mississippi";
    private const string NearRepeatText = "abcabd";
    private const string NestedRepeatText = "aabaabaaab";

    // Hand-verified worked example from the design pass: "abcbc" triggers two clone events,
    // including a clone becoming q in a later Extend call (exercising the walk-continues-from-a-
    // clone path). DistinctSubstringCount was independently cross-checked against brute-force
    // enumeration of all substrings of "abcbc".
    [Fact]
    public void DistinctSubstringCount_HandVerifiedWorkedExample_MatchesTracedValue()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        Assert.Equal(WorkedExampleDistinctSubstringCount, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void CountOccurrences_HandVerifiedWorkedExample_MatchesTracedSharedEndposState()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        // "c" (indices 2, 4) and "bc" (indices 1-2, 3-4) both land on the same traced state (7),
        // each occurring exactly twice.
        Assert.Equal(SharedEndposStateOccurrenceCount, automaton.CountOccurrences(TracedSubstringC));
        Assert.Equal(SharedEndposStateOccurrenceCount, automaton.CountOccurrences(TracedSubstringBc));
    }

    [Fact]
    public void HasSubstring_PresentSubstring_ReturnsTrue()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        Assert.True(automaton.HasSubstring(PresentSubstring));
    }

    [Fact]
    public void HasSubstring_AbsentSubstring_ReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        Assert.False(automaton.HasSubstring(AbsentSubstringXyz));
        Assert.False(automaton.HasSubstring(AbsentSubstringBa));
    }

    [Fact]
    public void CountOccurrences_AbsentPattern_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        Assert.Equal(0, automaton.CountOccurrences(AbsentSubstringXyz));
    }

    [Fact]
    public void CountOccurrences_EmptyPattern_ReturnsLengthPlusOne()
    {
        var automaton = new SuffixAutomatonStructure(WorkedExampleText);

        Assert.Equal(WorkedExampleEmptyPatternOccurrenceCount, automaton.CountOccurrences(EmptyString));
    }

    [Fact]
    public void HasSubstring_EmptyPattern_ReturnsTrueEvenForEmptyText()
    {
        var automaton = new SuffixAutomatonStructure(EmptyString);

        Assert.True(automaton.HasSubstring(EmptyString));
    }

    [Fact]
    public void HasSubstring_EmptyText_NonEmptyPatternReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure(EmptyString);

        Assert.False(automaton.HasSubstring(SingleCharacterPattern));
    }

    [Fact]
    public void DistinctSubstringCount_EmptyText_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure(EmptyString);

        Assert.Equal(0, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_AllDistinctCharacters_EqualsTriangularNumberOfLength()
    {
        var automaton = new SuffixAutomatonStructure(AllDistinctCharactersText);

        // n distinct-character text has exactly n*(n+1)/2 distinct substrings (every substring is
        // automatically unique when no character repeats).
        Assert.Equal(TriangularSubstringCountForLengthFive, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_MatchesBruteForceEnumerationAcrossVariedInputs()
    {
        string[] texts = [WorkedExampleText, BananaText, RepeatedACharacterText, MississippiText, NearRepeatText, NestedRepeatText];

        foreach (var text in texts)
        {
            var automaton = new SuffixAutomatonStructure(text);

            Assert.Equal(BruteForceDistinctSubstringCount(text), automaton.DistinctSubstringCount);
        }
    }

    [Fact]
    public void CountOccurrences_MatchesBruteForceOccurrenceCountAcrossVariedSubstrings()
    {
        string[] texts = [WorkedExampleText, BananaText, RepeatedACharacterText, MississippiText, NearRepeatText];

        foreach (var text in texts)
        {
            var automaton = new SuffixAutomatonStructure(text);

            foreach (var substring in AllDistinctSubstrings(text))
            {
                var expected = BruteForceCountOccurrences(text, substring);

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

    private static int BruteForceCountOccurrences(string text, string pattern)
    {
        var count = 0;

        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (text.AsSpan(start, pattern.Length).SequenceEqual(pattern))
            {
                count++;
            }
        }

        return count;
    }
}
