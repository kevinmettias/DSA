using DSAExperimentation.LeetCode.ShortestMatchingSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestMatchingSubstring;

// Harness only. Splitting p and finding the shortest match are both
// ShortestMatchingSubstringSolution's - this file just pins both strategies to
// LeetCode's published examples, including the all-empty-parts and no-match cases.
public sealed class ShortestMatchingSubstringTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "abaacbaecebce", "ba*c*ce", 8 },
            { "baccbaadbc", "cc*baa*adb", -1 },
            { "a", "**", 0 },
            { "madlogic", "*adlogi*", 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestLengthByBruteForceIndexOf_LeetCodeExamples_ReturnsShortestMatchLength(
        string s, string p, int expected) =>
        Assert.Equal(expected, ShortestMatchingSubstringSolution.ShortestLengthByBruteForceIndexOf(s, p));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestLengthByKmpBinarySearch_LeetCodeExamples_ReturnsShortestMatchLength(
        string s, string p, int expected) =>
        Assert.Equal(expected, ShortestMatchingSubstringSolution.ShortestLengthByKmpBinarySearch(s, p));
}
