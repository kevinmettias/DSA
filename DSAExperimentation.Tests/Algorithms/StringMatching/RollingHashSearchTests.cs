using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.Algorithms.StringMatching;

public sealed partial class RollingHashSearchTests
{
    // Direct parity against the repo's other FindAll (PrefixFunctionSearch/KMP),
    // reusing its own scenarios plus two more varied ones - two
    // independently-derived algorithms agreeing is stronger evidence than either
    // one's own hand-verified examples alone.
    [Fact]
    public void FindAll_PatternPresentOnce_MatchesPrefixFunctionSearch()
    {
        var expected = PrefixFunctionSearch.FindAll("hello world", "world");
        var actual = RollingHashSearch.FindAll("hello world", "world");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindAll_PatternPresentMultipleTimesOverlapping_MatchesPrefixFunctionSearch()
    {
        var expected = PrefixFunctionSearch.FindAll("aaaa", "aa");
        var actual = RollingHashSearch.FindAll("aaaa", "aa");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindAll_PatternAbsent_MatchesPrefixFunctionSearch()
    {
        var expected = PrefixFunctionSearch.FindAll("abcdef", "xyz");
        var actual = RollingHashSearch.FindAll("abcdef", "xyz");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindAll_RepeatedOverlappingSubstring_MatchesPrefixFunctionSearch()
    {
        var expected = PrefixFunctionSearch.FindAll("mississippi", "issi");
        var actual = RollingHashSearch.FindAll("mississippi", "issi");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindAll_AlternatingPattern_MatchesPrefixFunctionSearch()
    {
        var expected = PrefixFunctionSearch.FindAll("abababab", "aba");
        var actual = RollingHashSearch.FindAll("abababab", "aba");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindAll_EmptyPattern_MatchesEveryInsertionPoint()
    {
        var matches = RollingHashSearch.FindAll("abc", "");

        Assert.Equal([0, 1, 2, 3], matches);
    }

    [Fact]
    public void FindAll_PatternLongerThanText_ReturnsEmptyList()
    {
        var matches = RollingHashSearch.FindAll("ab", "abc");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var matches = RollingHashSearch.FindAll("BaBaB", "bab", caseInsensitive);

        Assert.Equal([0, 2], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var matches = RollingHashSearch.FindAll("BaBaB", "bab");

        Assert.Empty(matches);
    }
}
