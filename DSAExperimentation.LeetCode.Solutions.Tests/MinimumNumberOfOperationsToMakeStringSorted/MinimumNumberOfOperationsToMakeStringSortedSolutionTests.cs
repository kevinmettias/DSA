using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfOperationsToMakeStringSorted;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfOperationsToMakeStringSorted;

// Harness only: both strategies live in
// MinimumNumberOfOperationsToMakeStringSortedSolution. One test method per strategy
// over one shared set of cases, so a failure names the strategy that broke - the
// linear frequency scan included, which was previously a benchmark-only arm nothing
// asserted. The cases are small strings whose permutation rank can be counted by
// hand: already sorted, a single character, one letter repeated, and short unsorted
// strings with and without repeated letters.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumNumberOfOperationsToMakeStringSortedSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "zyx", 5 }, // same shape as "cba", shifted up the alphabet
            { "abc", 0 }, // already sorted: rank 0, no operations
            { "q", 0 }, // single character
            { "aa", 0 }, // one distinct permutation only
            { "ba", 1 }, // ab, ba
            { "cdc", 1 }, // ccd, cdc, dcc
            { "bbaa", 5 }, // aabb, abab, abba, baab, baba, bbaa
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeStringSortedByFrequencyScan_Cases_ReturnsPermutationRank(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFrequencyScan(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeStringSortedByFenwickSweep_Cases_ReturnsPermutationRank(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFenwickSweep(text));
}
