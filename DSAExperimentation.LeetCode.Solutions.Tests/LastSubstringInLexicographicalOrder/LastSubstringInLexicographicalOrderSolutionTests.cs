using DSAExperimentation.LeetCode.Solutions.LastSubstringInLexicographicalOrder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LastSubstringInLexicographicalOrder;

// Harness only: both strategies live in LastSubstringInLexicographicalOrderSolution
// and are asserted against the same cases - LeetCode's own two, the single
// character, the all-equal string (where the tie between equal-prefixed suffixes must
// break on length), and a case whose maximal suffix starts at the LAST occurrence of
// the largest character rather than the first.
public sealed partial class LastSubstringInLexicographicalOrderSolutionTests
{
    public static TheoryData<MaximalSuffixCase> Cases =>
        new()
        {
            { new MaximalSuffixCase(S: "z", Expected: "z") },
            { new MaximalSuffixCase(S: "cacacb", Expected: "cb") },
            { new MaximalSuffixCase(S: "zzazz", Expected: "zzazz") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastSubstringByPairwiseComparison_Cases_ReturnsMaximalSuffix(
        MaximalSuffixCase testCase)
    {
        var actual = LastSubstringInLexicographicalOrderSolution.LastSubstringByPairwiseComparison(testCase.S);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastSubstringBySuffixArray_Cases_ReturnsMaximalSuffix(MaximalSuffixCase testCase)
    {
        var actual = LastSubstringInLexicographicalOrderSolution.LastSubstringBySuffixArray(testCase.S);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the string to take the maximal suffix of, and that suffix.
    // Both are `string` and the question is not symmetric - the suffix is drawn from
    // the string, never the other way round - so the row names the roles instead of
    // leaving two adjacent positions a caller could swap with the compiler silent.
    public readonly record struct MaximalSuffixCase(string S, string Expected);
}
