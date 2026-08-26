using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.Algorithms.StringMatching;

public sealed partial class PrefixFunctionSearchTests
{
    [Fact]
    public void ComputeFailureFunction_KnownPattern_ReturnsExpectedPrefixFunction()
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction("ababaca");

        Assert.Equal([0, 0, 1, 2, 3, 0, 1], failure);
    }

    [Fact]
    public void ComputeFailureFunction_NoRepeatedPrefix_ReturnsAllZeros()
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction("abcde");

        Assert.Equal([0, 0, 0, 0, 0], failure);
    }

    [Fact]
    public void ComputeFailureFunction_EmptyPattern_ReturnsEmptyArray()
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction("");

        Assert.Empty(failure);
    }

    [Fact]
    public void ComputeFailureFunction_WithCaseInsensitiveComparer_TreatsCharsAsEqual()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var failure = PrefixFunctionSearch.ComputeFailureFunction("AaAa", caseInsensitive);

        Assert.Equal([0, 1, 2, 3], failure);
    }

    [Fact]
    public void FindAll_PatternPresentOnce_ReturnsSingleIndex()
    {
        var matches = PrefixFunctionSearch.FindAll("hello world", "world");

        Assert.Equal([6], matches);
    }

    [Fact]
    public void FindAll_PatternPresentMultipleTimesOverlapping_ReturnsAllIndices()
    {
        var matches = PrefixFunctionSearch.FindAll("aaaa", "aa");

        Assert.Equal([0, 1, 2], matches);
    }

    [Fact]
    public void FindAll_PatternAbsent_ReturnsEmptyList()
    {
        var matches = PrefixFunctionSearch.FindAll("abcdef", "xyz");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_PatternLongerThanText_ReturnsEmptyList()
    {
        var matches = PrefixFunctionSearch.FindAll("ab", "abc");

        Assert.Empty(matches);
    }

    [Fact]
    public void FindAll_EmptyPattern_MatchesEveryInsertionPoint()
    {
        var matches = PrefixFunctionSearch.FindAll("abc", "");

        Assert.Equal([0, 1, 2, 3], matches);
    }

    [Fact]
    public void FindAll_WithCaseInsensitiveComparer_MatchesRegardlessOfCase()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var matches = PrefixFunctionSearch.FindAll("AbcABC", "abc", caseInsensitive);

        Assert.Equal([0, 3], matches);
    }

    [Fact]
    public void FindAll_WithDefaultComparer_IsCaseSensitive()
    {
        var matches = PrefixFunctionSearch.FindAll("AbcABC", "abc");

        Assert.Empty(matches);
    }
}
