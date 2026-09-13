using DSAExperimentation.LeetCode.LastSubstringInLexicographicalOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastSubstringInLexicographicalOrder;

// Harness only: both strategies live in LastSubstringInLexicographicalOrderSolution
// and are asserted against the same examples - LeetCode's own two, the single
// character, the all-equal string (where the tie between equal-prefixed suffixes must
// break on length), and a case whose maximal suffix starts at the LAST occurrence of
// the largest character rather than the first.
public sealed class LastSubstringInLexicographicalOrderTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "abab", "bab" },
            { "leetcode", "tcode" },
            { "z", "z" },
            { "aaaa", "aaaa" },
            { "cacacb", "cb" },
            { "zzazz", "zzazz" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastSubstringByPairwiseComparison_LeetCodeExamples_ReturnsMaximalSuffix(
        string s, string expected) =>
        Assert.Equal(
            expected,
            LastSubstringInLexicographicalOrderSolution.LastSubstringByPairwiseComparison(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastSubstringBySuffixArray_LeetCodeExamples_ReturnsMaximalSuffix(
        string s, string expected) =>
        Assert.Equal(
            expected,
            LastSubstringInLexicographicalOrderSolution.LastSubstringBySuffixArray(s));
}
