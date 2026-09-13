using DSAExperimentation.LeetCode.LongestDuplicateSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestDuplicateSubstring;

// Harness only: both strategies live in LongestDuplicateSubstringSolution and are
// asserted against the same examples - LeetCode's own two, the overlapping-repeat
// case, a non-overlapping repeat, and the two shortest inputs that can and cannot
// contain a duplicate at all. Every example has a UNIQUE longest duplicate, so both
// strategies must agree on the substring and not merely on its length.
public sealed class LongestDuplicateSubstringTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "banana", "ana" },
            { "abcd", "" },
            { "aaaaa", "aaaa" },
            { "abcabc", "abc" },
            { "aa", "a" },
            { "a", "" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDupSubstringByAllSuffixPairs_LeetCodeExamples_ReturnsLongestRepeatedSubstring(string s, string expected) =>
        Assert.Equal(expected, LongestDuplicateSubstringSolution.LongestDupSubstringByAllSuffixPairs(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDupSubstringBySuffixArray_LeetCodeExamples_ReturnsLongestRepeatedSubstring(string s, string expected) =>
        Assert.Equal(expected, LongestDuplicateSubstringSolution.LongestDupSubstringBySuffixArray(s));
}
