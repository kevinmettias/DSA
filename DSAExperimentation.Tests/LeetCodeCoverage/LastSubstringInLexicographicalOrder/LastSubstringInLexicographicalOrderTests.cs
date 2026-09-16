using DSAExperimentation.LeetCode.LastSubstringInLexicographicalOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastSubstringInLexicographicalOrder;

// Harness only: both strategies live in LastSubstringInLexicographicalOrderSolution
// and are asserted against the same examples - LeetCode's own two, the single
// character, the all-equal string (where the tie between equal-prefixed suffixes must
// break on length), and a case whose maximal suffix starts at the LAST occurrence of
// the largest character rather than the first.
public sealed class LastSubstringInLexicographicalOrderTests
{
    public static TheoryData<MaximalSuffixExample> Examples =>
        new()
        {
            { new MaximalSuffixExample(S: "abab", Expected: "bab") },
            { new MaximalSuffixExample(S: "leetcode", Expected: "tcode") },
            { new MaximalSuffixExample(S: "z", Expected: "z") },
            { new MaximalSuffixExample(S: "aaaa", Expected: "aaaa") },
            { new MaximalSuffixExample(S: "cacacb", Expected: "cb") },
            { new MaximalSuffixExample(S: "zzazz", Expected: "zzazz") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastSubstringByPairwiseComparison_LeetCodeExamples_ReturnsMaximalSuffix(
        MaximalSuffixExample example)
    {
        var actual = LastSubstringInLexicographicalOrderSolution.LastSubstringByPairwiseComparison(example.S);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastSubstringBySuffixArray_LeetCodeExamples_ReturnsMaximalSuffix(MaximalSuffixExample example)
    {
        var actual = LastSubstringInLexicographicalOrderSolution.LastSubstringBySuffixArray(example.S);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the string to take the maximal suffix of, and that suffix.
    // Both are `string` and the question is not symmetric - the suffix is drawn from
    // the string, never the other way round - so the row names the roles instead of
    // leaving two adjacent positions a caller could swap with the compiler silent.
    public readonly record struct MaximalSuffixExample(string S, string Expected);
}
