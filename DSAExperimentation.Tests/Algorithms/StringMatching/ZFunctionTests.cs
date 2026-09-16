using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.Algorithms.StringMatching;

public sealed partial class ZFunctionTests
{
    private static readonly (string Text, string Pattern)[] TextPatternCases =
    [
        ("hello world", "world"),
        ("aaaa", "aa"),
        ("aabaab", "aab"),
        ("aaaaa", "aaa"),
        ("mississippi", "issi"),
        ("abcdef", "xyz"),
    ];

    [Fact]
    public void Compute_KnownText_ReturnsExpectedZArray()
    {
        var z = ZFunction.Compute(Fixtures.KnownText);

        Assert.Equal([0, Fixtures.RepeatedPrefixLength, 1, 0, Fixtures.RepeatedPrefixLength, 1, 0], z);
    }

    [Fact]
    public void Compute_NoRepeatedPrefix_ReturnsAllZeros()
    {
        var z = ZFunction.Compute(Fixtures.NoRepeatedPrefixText);

        Assert.Equal([0, 0, 0, 0, 0], z);
    }

    [Fact]
    public void Compute_EmptyText_ReturnsEmptyArray()
    {
        var z = ZFunction.Compute(Fixtures.EmptyText);

        Assert.Empty(z);
    }

    [Fact]
    public void Compute_MatchesBruteForceLongestCommonPrefix()
    {
        var z = ZFunction.Compute(Fixtures.BruteForceText);

        for (var i = 1; i < Fixtures.BruteForceText.Length; i++)
        {
            var bruteForceLength = BruteForceLongestCommonPrefix(Fixtures.BruteForceText, Fixtures.BruteForceText.AsSpan(i));
            Assert.Equal(bruteForceLength, z[i]);
        }
    }

    [Fact]
    public void Compute_WithCaseInsensitiveComparer_TreatsCharsAsEqual()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var z = ZFunction.Compute(Fixtures.MixedCaseText, caseInsensitive);

        Assert.Equal([0, Fixtures.PrefixMatchAtIndex1, Fixtures.PrefixMatchAtIndex2, 1], z);
    }

    [Fact]
    public void FindAll_PatternPresentOnce_ReturnsSingleIndex()
    {
        var matches = ZFunction.FindAll(Fixtures.HelloWorldText, Fixtures.WorldPattern);

        Assert.Equal([Fixtures.MatchIndex], matches);
    }

    [Fact]
    public void FindAll_PatternPresentMultipleTimesOverlapping_ReturnsAllIndices()
    {
        var matches = ZFunction.FindAll(Fixtures.RepeatedPairText, Fixtures.RepeatedPairPattern);

        Assert.Equal([0, 1, Fixtures.LastMatchIndex], matches);
    }

    // Regression coverage for the mirror-reuse branch specifically: a self-overlapping pattern
    // ("aab" shares a length-1 proper prefix/suffix overlap with itself) searched against text
    // containing two separated occurrences. Non-repetitive fixtures like "hello world"/"world"
    // never exercise SeedFromPatternBox's mirror-reuse path at all.
    [Fact]
    public void FindAll_SelfOverlappingPattern_ReturnsAllRealOccurrencesWithNoPhantomMatches()
    {
        var matches = ZFunction.FindAll(Fixtures.SelfOverlappingText, Fixtures.SelfOverlappingPattern);

        Assert.Equal([0, Fixtures.SecondMatchIndex], matches);
    }

    [Fact]
    public void FindAll_HighlySelfOverlappingPattern_ReturnsAllOverlappingOccurrences()
    {
        var matches = ZFunction.FindAll(Fixtures.TripleRepeatText, Fixtures.TripleRepeatPattern);

        Assert.Equal([0, 1, Fixtures.LastMatchIndex], matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var matches = ZFunction.FindAll(Fixtures.AbsentText, Fixtures.AbsentPattern);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternLongerThanText_ReturnsEmptyList()
    {
        var matches = ZFunction.FindAll(Fixtures.ShortText, Fixtures.LongerPattern);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyPattern_MatchesEveryInsertionPoint()
    {
        var matches = ZFunction.FindAll(Fixtures.InsertionPointText, Fixtures.EmptyPattern);

        Assert.Equal([0, 1, Fixtures.ThirdInsertionPoint, Fixtures.FourthInsertionPoint], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var matches = ZFunction.FindAll(Fixtures.CaseVariantText, Fixtures.CaseVariantPattern, caseInsensitive);

        Assert.Equal([0, Fixtures.SecondMatchIndex], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var matches = ZFunction.FindAll(Fixtures.CaseVariantText, Fixtures.CaseVariantPattern);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_MatchesPrefixFunctionSearchAcrossVariedInputs()
    {
        foreach (var (text, pattern) in TextPatternCases)
        {
            var expected = PrefixFunctionSearch.FindAll(text, pattern);
            var actual = ZFunction.FindAll(text, pattern);

            Assert.Equal(expected, actual);
        }
    }

    private static int BruteForceLongestCommonPrefix(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        var length = 0;

        while (CharsMatchAt(first, second, length))
        {
            length++;
        }

        return length;
    }

    private static bool CharsMatchAt(ReadOnlySpan<char> first, ReadOnlySpan<char> second, int index)
    {
        if (index >= first.Length || index >= second.Length)
        {
            return false;
        }

        return first[index] == second[index];
    }

    /// <summary>
    /// The inputs these tests search through and the indices they expect back, named
    /// once so a second test does not have to reach into a neighbour's body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string KnownText = "aaabaab";
        public const int RepeatedPrefixLength = 2;
        public const string NoRepeatedPrefixText = "abcde";
        public const string EmptyText = "";
        public const string BruteForceText = "aabaabaaab";
        public const string MixedCaseText = "AaAa";
        public const int PrefixMatchAtIndex1 = 3;
        public const int PrefixMatchAtIndex2 = 2;
        public const string HelloWorldText = "hello world";
        public const string WorldPattern = "world";
        public const int MatchIndex = 6;
        public const string RepeatedPairText = "aaaa";
        public const string RepeatedPairPattern = "aa";
        public const int LastMatchIndex = 2;
        public const string SelfOverlappingText = "aabaab";
        public const string SelfOverlappingPattern = "aab";
        public const int SecondMatchIndex = 3;
        public const string TripleRepeatText = "aaaaa";
        public const string TripleRepeatPattern = "aaa";
        public const string AbsentText = "abcdef";
        public const string AbsentPattern = "xyz";
        public const string ShortText = "ab";
        public const string LongerPattern = "abc";
        public const string InsertionPointText = "abc";
        public const string EmptyPattern = "";
        public const int ThirdInsertionPoint = 2;
        public const int FourthInsertionPoint = 3;
        public const string CaseVariantText = "AbcABC";
        public const string CaseVariantPattern = "abc";
    }
}
