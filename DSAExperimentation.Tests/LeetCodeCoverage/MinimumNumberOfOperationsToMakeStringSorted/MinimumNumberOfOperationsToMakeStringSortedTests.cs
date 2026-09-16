using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeStringSorted;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfOperationsToMakeStringSorted;

// Harness only: both strategies live in
// MinimumNumberOfOperationsToMakeStringSortedSolution. One test method per strategy
// over one shared set of examples, so a failure names the strategy that broke - the
// linear frequency scan included, which was previously a benchmark-only arm nothing
// asserted.
public sealed class MinimumNumberOfOperationsToMakeStringSortedTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "cba", 5 }, // LC example 1: abc, acb, bac, bca, cab, cba
            { "aabaa", 2 }, // LC example 2: aaaab, aaaba, aabaa
            { "zyx", 5 }, // same shape as "cba", shifted up the alphabet
            { "abc", 0 }, // already sorted: rank 0, no operations
            { "q", 0 }, // single character
            { "aa", 0 }, // one distinct permutation only
            { "ba", 1 }, // ab, ba
            { "cdc", 1 }, // ccd, cdc, dcc
            { "bbaa", 5 }, // aabb, abab, abba, baab, baba, bbaa
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeStringSortedByFrequencyScan_LeetCodeExamples_ReturnsPermutationRank(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFrequencyScan(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeStringSortedByFenwickSweep_LeetCodeExamples_ReturnsPermutationRank(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFenwickSweep(text));
}
