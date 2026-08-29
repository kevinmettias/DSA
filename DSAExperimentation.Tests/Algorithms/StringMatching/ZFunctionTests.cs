using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.Algorithms.StringMatching;

public sealed partial class ZFunctionTests
{
    [Fact]
    public void Compute_KnownText_ReturnsExpectedZArray()
    {
        var z = ZFunction.Compute("aaabaab");

        Assert.Equal([0, 2, 1, 0, 2, 1, 0], z);
    }

    [Fact]
    public void Compute_NoRepeatedPrefix_ReturnsAllZeros()
    {
        var z = ZFunction.Compute("abcde");

        Assert.Equal([0, 0, 0, 0, 0], z);
    }

    [Fact]
    public void Compute_EmptyText_ReturnsEmptyArray()
    {
        var z = ZFunction.Compute("");

        Assert.Empty(z);
    }

    [Fact]
    public void Compute_MatchesBruteForceLongestCommonPrefix()
    {
        const string text = "aabaabaaab";

        var z = ZFunction.Compute(text);

        for (var i = 1; i < text.Length; i++)
        {
            Assert.Equal(BruteForceLongestCommonPrefix(text, text.AsSpan(i)), z[i]);
        }
    }

    [Fact]
    public void Compute_WithCaseInsensitiveComparer_TreatsCharsAsEqual()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var z = ZFunction.Compute("AaAa", caseInsensitive);

        Assert.Equal([0, 3, 2, 1], z);
    }

    [Fact]
    public void FindAll_PatternPresentOnce_ReturnsSingleIndex()
    {
        var matches = ZFunction.FindAll("hello world", "world");

        Assert.Equal([6], matches);
    }

    [Fact]
    public void FindAll_PatternPresentMultipleTimesOverlapping_ReturnsAllIndices()
    {
        var matches = ZFunction.FindAll("aaaa", "aa");

        Assert.Equal([0, 1, 2], matches);
    }

    // Regression coverage for the mirror-reuse branch specifically: a self-overlapping pattern
    // ("aab" shares a length-1 proper prefix/suffix overlap with itself) searched against text
    // containing two separated occurrences. Non-repetitive fixtures like "hello world"/"world"
    // never exercise SeedFromPatternBox's mirror-reuse path at all.
    [Fact]
    public void FindAll_SelfOverlappingPattern_ReturnsAllRealOccurrencesWithNoPhantomMatches()
    {
        var matches = ZFunction.FindAll("aabaab", "aab");

        Assert.Equal([0, 3], matches);
    }

    [Fact]
    public void FindAll_HighlySelfOverlappingPattern_ReturnsAllOverlappingOccurrences()
    {
        var matches = ZFunction.FindAll("aaaaa", "aaa");

        Assert.Equal([0, 1, 2], matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var matches = ZFunction.FindAll("abcdef", "xyz");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternLongerThanText_ReturnsEmptyList()
    {
        var matches = ZFunction.FindAll("ab", "abc");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyPattern_MatchesEveryInsertionPoint()
    {
        var matches = ZFunction.FindAll("abc", "");

        Assert.Equal([0, 1, 2, 3], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var matches = ZFunction.FindAll("AbcABC", "abc", caseInsensitive);

        Assert.Equal([0, 3], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var matches = ZFunction.FindAll("AbcABC", "abc");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_MatchesPrefixFunctionSearchAcrossVariedInputs()
    {
        (string Text, string Pattern)[] cases =
        [
            ("hello world", "world"),
            ("aaaa", "aa"),
            ("aabaab", "aab"),
            ("aaaaa", "aaa"),
            ("mississippi", "issi"),
            ("abcdef", "xyz"),
        ];

        foreach (var (text, pattern) in cases)
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
