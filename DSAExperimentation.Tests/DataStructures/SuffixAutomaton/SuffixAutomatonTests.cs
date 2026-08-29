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
        var automaton = new SuffixAutomatonStructure("abcbc");

        Assert.Equal(12, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void CountOccurrences_HandVerifiedWorkedExample_MatchesTracedSharedEndposState()
    {
        var automaton = new SuffixAutomatonStructure("abcbc");

        // "c" (indices 2, 4) and "bc" (indices 1-2, 3-4) both land on the same traced state (7),
        // each occurring exactly twice.
        Assert.Equal(2, automaton.CountOccurrences("c"));
        Assert.Equal(2, automaton.CountOccurrences("bc"));
    }

    [Fact]
    public void HasSubstring_PresentSubstring_ReturnsTrue()
    {
        var automaton = new SuffixAutomatonStructure("abcbc");

        Assert.True(automaton.HasSubstring("cbc"));
    }

    [Fact]
    public void HasSubstring_AbsentSubstring_ReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure("abcbc");

        Assert.False(automaton.HasSubstring("xyz"));
        Assert.False(automaton.HasSubstring("ba"));
    }

    [Fact]
    public void CountOccurrences_AbsentPattern_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure("abcbc");

        Assert.Equal(0, automaton.CountOccurrences("xyz"));
    }

    [Fact]
    public void CountOccurrences_EmptyPattern_ReturnsLengthPlusOne()
    {
        var automaton = new SuffixAutomatonStructure("abcbc");

        Assert.Equal(6, automaton.CountOccurrences(""));
    }

    [Fact]
    public void HasSubstring_EmptyPattern_ReturnsTrueEvenForEmptyText()
    {
        var automaton = new SuffixAutomatonStructure("");

        Assert.True(automaton.HasSubstring(""));
    }

    [Fact]
    public void HasSubstring_EmptyText_NonEmptyPatternReturnsFalse()
    {
        var automaton = new SuffixAutomatonStructure("");

        Assert.False(automaton.HasSubstring("a"));
    }

    [Fact]
    public void DistinctSubstringCount_EmptyText_ReturnsZero()
    {
        var automaton = new SuffixAutomatonStructure("");

        Assert.Equal(0, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_AllDistinctCharacters_EqualsTriangularNumberOfLength()
    {
        var automaton = new SuffixAutomatonStructure("abcde");

        // n distinct-character text has exactly n*(n+1)/2 distinct substrings (every substring is
        // automatically unique when no character repeats).
        Assert.Equal(15, automaton.DistinctSubstringCount);
    }

    [Fact]
    public void DistinctSubstringCount_MatchesBruteForceEnumerationAcrossVariedInputs()
    {
        string[] texts = ["abcbc", "banana", "aaaa", "mississippi", "abcabd", "aabaabaaab"];

        foreach (var text in texts)
        {
            var automaton = new SuffixAutomatonStructure(text);

            Assert.Equal(BruteForceDistinctSubstringCount(text), automaton.DistinctSubstringCount);
        }
    }

    [Fact]
    public void CountOccurrences_MatchesBruteForceOccurrenceCountAcrossVariedSubstrings()
    {
        string[] texts = ["abcbc", "banana", "aaaa", "mississippi", "abcabd"];

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
                substrings.Add(text.Substring(start, length));
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
