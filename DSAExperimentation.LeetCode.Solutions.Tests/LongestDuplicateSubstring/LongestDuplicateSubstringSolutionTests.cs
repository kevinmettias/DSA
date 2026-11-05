using DSAExperimentation.LeetCode.Solutions.LongestDuplicateSubstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestDuplicateSubstring;

// Harness only: both strategies live in LongestDuplicateSubstringSolution and are
// asserted against the same cases - a non-overlapping repeat and the two shortest
// inputs that can and cannot contain a duplicate at all. Every case has a UNIQUE
// longest duplicate, so both strategies must agree on the substring and not merely
// on its length. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LongestDuplicateSubstringSolutionTests
{
    public static TheoryData<DuplicateSubstringCase> Cases =>
        new()
        {
            { new DuplicateSubstringCase(S: "abcabc", Expected: "abc") },
            { new DuplicateSubstringCase(S: "aa", Expected: "a") },
            { new DuplicateSubstringCase(S: "a", Expected: "") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestDuplicateSubstringByAllSuffixPairs_Cases_ReturnsLongestRepeatedSubstring(
        DuplicateSubstringCase testCase) =>
        Assert.Equal(testCase.Expected, LongestDuplicateSubstringSolution.LongestDuplicateSubstringByAllSuffixPairs(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestDuplicateSubstringBySuffixArray_Cases_ReturnsLongestRepeatedSubstring(
        DuplicateSubstringCase testCase) =>
        Assert.Equal(testCase.Expected, LongestDuplicateSubstringSolution.LongestDuplicateSubstringBySuffixArray(testCase.S));

    // One case as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct DuplicateSubstringCase(string S, string Expected);
}
