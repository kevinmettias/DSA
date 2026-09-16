using DSAExperimentation.LeetCode.ShortestMatchingSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestMatchingSubstring;

// Harness only. Splitting p and finding the shortest match are both
// ShortestMatchingSubstringSolution's - this file just pins both strategies to
// LeetCode's published examples, including the all-empty-parts and no-match cases.
public sealed class ShortestMatchingSubstringTests
{
    public static TheoryData<WildcardMatchExample> Examples =>
        new()
        {
            new WildcardMatchExample(Source: "abaacbaecebce", Pattern: "ba*c*ce", Expected: 8),
            new WildcardMatchExample(Source: "baccbaadbc", Pattern: "cc*baa*adb", Expected: -1),
            new WildcardMatchExample(Source: "a", Pattern: "**", Expected: 0),
            new WildcardMatchExample(Source: "madlogic", Pattern: "*adlogi*", Expected: 6),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestLengthByBruteForceIndexOf_LeetCodeExamples_ReturnsShortestMatchLength(
        WildcardMatchExample example)
    {
        var actual = ShortestMatchingSubstringSolution.ShortestLengthByBruteForceIndexOf(
            new ShortestMatchingSubstringSolution.SourceText(example.Source),
            new ShortestMatchingSubstringSolution.WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestLengthByKmpBinarySearch_LeetCodeExamples_ReturnsShortestMatchLength(
        WildcardMatchExample example)
    {
        var actual = ShortestMatchingSubstringSolution.ShortestLengthByKmpBinarySearch(
            new ShortestMatchingSubstringSolution.SourceText(example.Source),
            new ShortestMatchingSubstringSolution.WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the source text, the '*' pattern to match within it, and
    // the shortest matching length (-1 when nothing matches). Source and pattern are
    // adjacent strings at the call site, so the bundle names which is which.
    public readonly record struct WildcardMatchExample(string Source, string Pattern, int Expected);
}
