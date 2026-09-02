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
        const string Text = "aaabaab";
        const int RepeatedPrefixLength = 2;

        var z = ZFunction.Compute(Text);

        Assert.Equal([0, RepeatedPrefixLength, 1, 0, RepeatedPrefixLength, 1, 0], z);
    }

    [Fact]
    public void Compute_NoRepeatedPrefix_ReturnsAllZeros()
    {
        const string Text = "abcde";

        var z = ZFunction.Compute(Text);

        Assert.Equal([0, 0, 0, 0, 0], z);
    }

    [Fact]
    public void Compute_EmptyText_ReturnsEmptyArray()
    {
        const string EmptyText = "";

        var z = ZFunction.Compute(EmptyText);

        Assert.Empty(z);
    }

    [Fact]
    public void Compute_MatchesBruteForceLongestCommonPrefix()
    {
        const string text = "aabaabaaab";

        var z = ZFunction.Compute(text);

        for (var i = 1; i < text.Length; i++)
        {
            var bruteForceLength = BruteForceLongestCommonPrefix(text, text.AsSpan(i));
            Assert.Equal(bruteForceLength, z[i]);
        }
    }

    [Fact]
    public void Compute_WithCaseInsensitiveComparer_TreatsCharsAsEqual()
    {
        const string Text = "AaAa";
        const int PrefixMatchAtIndex1 = 3;
        const int PrefixMatchAtIndex2 = 2;

        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var z = ZFunction.Compute(Text, caseInsensitive);

        Assert.Equal([0, PrefixMatchAtIndex1, PrefixMatchAtIndex2, 1], z);
    }

    [Fact]
    public void FindAll_PatternPresentOnce_ReturnsSingleIndex()
    {
        const string Text = "hello world";
        const string Pattern = "world";
        const int MatchIndex = 6;

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Equal([MatchIndex], matches);
    }

    [Fact]
    public void FindAll_PatternPresentMultipleTimesOverlapping_ReturnsAllIndices()
    {
        const string Text = "aaaa";
        const string Pattern = "aa";
        const int LastMatchIndex = 2;

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Equal([0, 1, LastMatchIndex], matches);
    }

    // Regression coverage for the mirror-reuse branch specifically: a self-overlapping pattern
    // ("aab" shares a length-1 proper prefix/suffix overlap with itself) searched against text
    // containing two separated occurrences. Non-repetitive fixtures like "hello world"/"world"
    // never exercise SeedFromPatternBox's mirror-reuse path at all.
    [Fact]
    public void FindAll_SelfOverlappingPattern_ReturnsAllRealOccurrencesWithNoPhantomMatches()
    {
        const string Text = "aabaab";
        const string Pattern = "aab";
        const int SecondMatchIndex = 3;

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Equal([0, SecondMatchIndex], matches);
    }

    [Fact]
    public void FindAll_HighlySelfOverlappingPattern_ReturnsAllOverlappingOccurrences()
    {
        const string Text = "aaaaa";
        const string Pattern = "aaa";
        const int LastMatchIndex = 2;

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Equal([0, 1, LastMatchIndex], matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        const string Text = "abcdef";
        const string Pattern = "xyz";

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternLongerThanText_ReturnsEmptyList()
    {
        const string Text = "ab";
        const string Pattern = "abc";

        var matches = ZFunction.FindAll(Text, Pattern);

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyPattern_MatchesEveryInsertionPoint()
    {
        const string Text = "abc";
        const string EmptyPattern = "";
        const int ThirdInsertionPoint = 2;
        const int FourthInsertionPoint = 3;

        var matches = ZFunction.FindAll(Text, EmptyPattern);

        Assert.Equal([0, 1, ThirdInsertionPoint, FourthInsertionPoint], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        const string Text = "AbcABC";
        const string Pattern = "abc";
        const int SecondMatchIndex = 3;

        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var matches = ZFunction.FindAll(Text, Pattern, caseInsensitive);

        Assert.Equal([0, SecondMatchIndex], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        const string Text = "AbcABC";
        const string Pattern = "abc";

        var matches = ZFunction.FindAll(Text, Pattern);

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

        while (length < first.Length && length < second.Length && first[length] == second[length])
        {
            length++;
        }

        return length;
    }
}
