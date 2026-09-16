using DSAExperimentation.LeetCode.LongestDuplicateSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestDuplicateSubstring;

// Harness only: both strategies live in LongestDuplicateSubstringSolution and are
// asserted against the same examples - LeetCode's own two, the overlapping-repeat
// case, a non-overlapping repeat, and the two shortest inputs that can and cannot
// contain a duplicate at all. Every example has a UNIQUE longest duplicate, so both
// strategies must agree on the substring and not merely on its length.
public sealed class LongestDuplicateSubstringTests
{
    public static TheoryData<DuplicateSubstringExample> Examples =>
        new()
        {
            { new DuplicateSubstringExample(S: "banana", Expected: "ana") },
            { new DuplicateSubstringExample(S: "abcd", Expected: "") },
            { new DuplicateSubstringExample(S: "aaaaa", Expected: "aaaa") },
            { new DuplicateSubstringExample(S: "abcabc", Expected: "abc") },
            { new DuplicateSubstringExample(S: "aa", Expected: "a") },
            { new DuplicateSubstringExample(S: "a", Expected: "") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDuplicateSubstringByAllSuffixPairs_LeetCodeExamples_ReturnsLongestRepeatedSubstring(
        DuplicateSubstringExample example) =>
        Assert.Equal(example.Expected, LongestDuplicateSubstringSolution.LongestDuplicateSubstringByAllSuffixPairs(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDuplicateSubstringBySuffixArray_LeetCodeExamples_ReturnsLongestRepeatedSubstring(
        DuplicateSubstringExample example) =>
        Assert.Equal(example.Expected, LongestDuplicateSubstringSolution.LongestDuplicateSubstringBySuffixArray(example.S));

    // One example as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct DuplicateSubstringExample(string S, string Expected);
}
